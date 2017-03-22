/** ETML
 * \file      WebIndexation.cs   
 * \author    Merk Yann
 * \brief     Contains the class used for web indexation
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

namespace P_OO_merkya_SmartThesaurus
{
    /// <summary>
    /// 
    /// </summary>
    /// \todo
    /// \todo Images / PDF?
    public class WebIndexation : IIndexation
    {
        /// <summary>
        /// List the markups containing unwanted text, to remove with the text
        /// </summary>
        /// <see cref="https://regex101.com/r/bnym7H/3"/>
        private readonly Regex CONTENT_MARKUP_BLACKLIST = new Regex(@"<script[^>]*>[^<]*</script>|<style[^>]*>[^<]*</style>|<!--.+?(-->)|<head>.+?(</head>)|<!DOCTYPE[^>]*>|<html[^>]*>|</html>|<body[^>]*>|</body>|<div[^>]*>|</div>|<form[^>]*>|</form>|<label[^>]*>|</label>|<input[^>]*>|</input>|<button[^>]*>|</button>|<a[^>]*>|</a>|<ul[^>]*>|</ul>|<ol[^>]*>|</ol>|<li[^>]*>|</li>|<img[^>]*>|<h[1-6][^>]*>|</h[1-6]>|<span[^>]*>|</span>|<strong[^>]*>|</strong>|<bold[^>]*>|</bold>|<strike[^>]*>|</strike>|<p[^>]*>|</p>", RegexOptions.Singleline);

        /// <summary>
        /// List all the links for other pages / object with an href= or src= tag
        /// </summary>
        private readonly Regex LINK_FINDER = new Regex("(href|src)=\"[^\"]+");

        /// <summary>
        /// List all the extension from which we can read the file content
        /// </summary>
        private readonly Regex READABLE_EXTENSION = new Regex(@"(/[^/.]*|.html|.php.?|.aspx)$");

        //Get the entire http://www.Domain.XX, or just Domain.XX in the group nb 1
        //http://stackoverflow.com/a/569189
        private readonly Regex DOMAIN_FINDER = new Regex(@"^(?:http[s]?://)?(?:www\.)?(.*?\.[^/]*)");

        /// <summary>
        /// Start the indexation of the website applying the mode inputed
        /// </summary>
        /// \todo Handle the type + maybe create more folders
        /// \todo Handle website not reachable
        /// <param name="_url">The url of the website from which get the content </param>
        /// <param name="_type">The type of exploration wanted</param>
        public void index(string _url, FileExplorationType _type)
        {
            //Check if the URL is valid
            //http://stackoverflow.com/a/5926605

            bool urlIsCorrect = false;

            WebRequest webRequest = WebRequest.Create(_url);
            WebResponse webResponse;
            try
            {
                webResponse = webRequest.GetResponse();
                urlIsCorrect = true;
            }
            catch
            { }

            //If the page can be reach
            if (urlIsCorrect)
            {
                //make sure there's a / at the end of the link
                if (_url.Split('/').Last() != "")
                {
                    _url += '/';
                }

                //add the root folder to the DB
                DB database = DB.getInstance();

                string idParentFolder = database.addFolder(_url, "0", FileOrigin.Web);

                //Start the indexation
                #region Indexation

                //Initialise the web client
                WebClient wc = new WebClient();

                //Create an hashset used to explore all the links between the pages, and add the current page to it
                HashSet<string> linksHashSet = new HashSet<string>();

                linksHashSet.Add(_url);


                string domain = DOMAIN_FINDER.Match(_url).Groups[1].Value;


                string filTextContent = "";

                //Loop until the end of the hashset
                for (int i = 0; i < linksHashSet.Count; i++)
                {
                    //check if this is a readablepage or not
                    if (READABLE_EXTENSION.IsMatch(linksHashSet.ElementAt(i)))
                    {
                        //try to read the content of the page
                        try
                        {
                            filTextContent = wc.DownloadString(linksHashSet.ElementAt(i));
                        }
                        catch
                        {
                            filTextContent = "";
                        }

                        //Parse only the text, and fill the hashet with links if the page contains text
                        if (filTextContent != "")
                        {
                            filTextContent = readPageContent(domain, filTextContent, ref linksHashSet);


                            //get the file name
                            string[] filSplittedName = linksHashSet.ElementAt(i).Split(new char[] { '/', '.' });


                            //else simply add it to the file list
                            string filName = linksHashSet.ElementAt(i);

                            //check if this links start with a domain or no
                            if (filName[0] != '/')
                            {
                                //remove the domain and stock the file name
                                filName = DOMAIN_FINDER.Replace(filName, "");
                            }

                            string filExtension = "";

                            filName = filName.Split(new char[] { '.' }, 2)[0];


                            database.addFile(filName, filExtension, idParentFolder, filTextContent);
                        }
                    }
                    else
                    {
                        //else simply add it to the file list
                        string filName = linksHashSet.ElementAt(i);

                        //check if this links start with a domain or no
                        if (filName[0] != '/')
                        {
                            //remove the domain and stock the file name
                            filName = DOMAIN_FINDER.Replace(filName, "");
                        }


                        //split the extension
                        string filExtension = filName.Split(new char[] { '.' }, 2)[1];

                        filName = filName.Split(new char[] { '.' }, 2)[0];

                        database.addFile(filName, filExtension, idParentFolder);
                    }
                }
                #endregion
            }
            else
            {
                //TODO display an error
            }
        }

        /// <summary>
        /// Get or create a folder to host the file inputed, and return his ID
        /// </summary>
        /// \todo
        /// <param name="_fileLocation">The path of the file</param>
        /// <returns>The ID of the folder in which to put the file</returns>
        private string getFolderID(string _fileLocation, string _idParentFolder)
        {
            //First remove the first part of the URL, if required
            if (_fileLocation[0] != '/')
            {
                _fileLocation = DOMAIN_FINDER.Replace(_fileLocation, "");
            }

            //If there's more than 1 "part" (separated by /) in the path, recurse
            if (_fileLocation.Split('/').Count() > 3)
            {
                //recurse without the last part
                //TODO
            }

            //Try to find the folder
            DB database = DB.getInstance();
            string folderID = database.searchFolder()
        }

        /// <summary>
        /// Read the content of an HTML (like) file to output the links into the hashset, and return the text content, markup free
        /// </summary>
        /// <param name="_pageContent">The HTML source content of the file</param>
        /// <param name="_linksHashSet">The hashset where to add the link toother pages</param>
        /// <returns>The "human readable" page content, free of markups</returns>
        private string readPageContent(string _domain, string _pageContent, ref HashSet<string> _linksHashSet)
        {

            //read the links
            readAllLink(_domain, _pageContent, ref _linksHashSet);

            //Remove all the markups
            _pageContent = CONTENT_MARKUP_BLACKLIST.Replace(_pageContent, "");

            //replace all the " by ""
            _pageContent = _pageContent.Replace("\"", "\"\"");

            //return it
            return _pageContent;
        }

        /// <summary>
        /// Read the content of an HTML (like) file to output the links into the hashset, and add all the links in it to the inputed hashset
        /// </summary>
        /// <param name="_domain">The domain of the page, used to make sure that we don't leave it</param>
        /// <param name="_pageContent">The HTML source content of the file</param>
        /// <param name="_linksHashSet">The hashset where to add the link toother pages</param>
        /// <returns>The "human readable" page content, free of markups</returns>
        private void readAllLink(string _domain, string _pageContent, ref HashSet<string> _linksHashSet)
        {
            Regex isLocalURL = new Regex(@"^(/|(http[s]?://)?(www\.)?" + _domain + @").*");


            //Get the links
            MatchCollection tempResult = LINK_FINDER.Matches(_pageContent);

            //go through all the matches
            foreach (Match match in tempResult)
            {
                //remove the href part
                string tempLink = match.Value.Split(new char[] { '"' }, 2)[1];

                //remove the www.part
                tempLink = tempLink.Replace("www.", "");

                //check if the link stays on the target website
                if (isLocalURL.IsMatch(tempLink))
                {
                    //Remove the href part, and add it to the hashset
                    _linksHashSet.Add(tempLink);
                }
            }
        }
    }
}