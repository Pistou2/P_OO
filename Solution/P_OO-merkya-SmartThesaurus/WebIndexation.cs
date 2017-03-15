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
        /// List all the links for other pages with an href= tag
        /// </summary>
        private readonly Regex LINK_FINDER = new Regex("href=\"[^\"]+");

        /// <summary>
        /// Start the indexation of the website applying the mode inputed
        /// </summary>
        /// \todo Handle the type + maybe create more folders
        /// \todo Handle website not reachable
        /// <param name="_url">The url of the website from which get the content</param>
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

                string pageContent = "";

                //Loop until the end of the hashset
                for (int i = 0; i < linksHashSet.Count; i++)
                {
                    //try to read the content of the page
                    try
                    {
                        pageContent = wc.DownloadString(linksHashSet.ElementAt(i));
                    }
                    catch
                    {
                        pageContent = "";
                    }

                    //Parse only the text, and fill the hashet with links if the page contains text
                    if (pageContent != "")
                    {
                        pageContent = readPageContent(pageContent, ref linksHashSet);
                    }

                }

                #endregion
            }
            else
            {
                //TODO
            }
        }

        /// <summary>
        /// Read the content of an HTML (like) file to output the links into the hashset, and return the text content, markup free
        /// </summary>
        /// <param name="_pageContent">The HTML source content of the file</param>
        /// <param name="_linksHashSet">The hashset where to add the link toother pages</param>
        /// <returns>The "human readable" page content, free of markups</returns>
        private string readPageContent(string _pageContent, ref HashSet<string> _linksHashSet)
        {
            //read the links
        }

        /// <summary>
        /// Read the content of an HTML (like) file to output the links into the hashset, and add all the links in it to the inputed hashset
        /// </summary>
        /// <param name="_websiteURL">The url of the website, used to make sure that we don't leave it</param>
        /// <param name="_pageContent">The HTML source content of the file</param>
        /// <param name="_linksHashSet">The hashset where to add the link toother pages</param>
        /// <returns>The "human readable" page content, free of markups</returns>
        private void readAllLink(string _websiteURL, string _pageContent, ref HashSet<string> _linksHashSet)
        {
            //Prepare the Regex used to make sure we stay on the website

            Regex isLocalURL = new Regex("")

            //Get the links
            MatchCollection tempResult = LINK_FINDER.Matches(_pageContent);

            //go through all the matches
            foreach (Match match in tempResult)
            {
                //remove the href part
                string tempLink = match.Value.Split(new char[] { '"' }, 2)[1];

                //check if the link stays on the target website
                if()


                //Remove the href part, and add it to the hashset
                _linkHashSet.Add(tempLink);
            }
        }
    }
}