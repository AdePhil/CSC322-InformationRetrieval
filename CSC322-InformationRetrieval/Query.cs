﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CSC322_InformationRetrieval
{
    /// <summary>
    /// The Query class.
    /// Queries strings against the inverted index. It is used by the Search class.
    /// </summary>
    public class Query
    {
        private readonly string queryString;
        private readonly InvertedIndex invertedIndex;

        /// <summary>
        ///A two arguement query contructor
        /// </summary>
        /// <param name="queryString">The string to be queried</param>
        /// <param name="queryPath">Path to unserialize the inverted index from</param>
        public Query(string queryString, string queryPath)
        {
            this.queryString = queryString;
            invertedIndex = new Serializer<InvertedIndex>(queryPath).Deserialize();
        }

        /// <summary>
        /// Queries the inverted index
        /// </summary>
        /// <returns>Returns the document ids that meets the query</returns>
        public SortedSet<int> QueryString()
        {
            //Handles one word and free text query.
            List<SortedSet<int>> eachWordsDocIds = new List<SortedSet<int>>();
            SortedSet<int> result = new SortedSet<int>();
            var wordsInQuery = EachTermSearched();
            //separate each word in the query
            for (int index = 0; index < wordsInQuery.Length; index++)
            {
                eachWordsDocIds.Add(new SortedSet<int>());
                var term = wordsInQuery[index];
                if (invertedIndex.ContainTerm(term)) //if the term is present in the invertedIndex
                {
                    foreach (var tuple in invertedIndex[term])
                    {
                        eachWordsDocIds[index].Add(tuple.DocumentId);
                        //get the docIds of in the postings list of the term
                    }
                }
                result.UnionWith(eachWordsDocIds[index]); //Union the idList of each term with the final result.
            }
            return result;
        }

        private string WordToTerm(string word)
        {
            return new PorterStemmer().StemWord(word.ToLower());
        }

        /// <summary>
        /// Returns the invertedIndex that was queried 
        /// </summary>
        /// <returns>Returns the inverted index object</returns>
        public InvertedIndex GetInvertedIndex()
        {
            return invertedIndex;
        }

        /// <summary>
        /// Splits each token in the searched string 
        /// </summary>
        /// <returns>Returns an of each token in the queried string</returns>
        public string[] EachTermSearched()
        {
            string[] result = queryString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            return result.Select(WordToTerm).ToArray();
        }
    }
}