using System;
using System.Collections.Generic;
using System.Linq;

class MainClass {
	// The dictionary to check the words against. Normally this would be loaded from a file.
	// If the dictionary is too big, loading it directly from the file into a trie would be better.
	private static string[] dictArray = new string[] {
		"all",
		"base",
		"ball",
		"baseball",
		"game",
		"ballgame" // Not a real word, used for testing.
	};
	
	public static void Main () {
		// Print the result of a couple of tests. If the test returns null, print "null" instead.
		Console.WriteLine("Array dictionary:");
		Console.WriteLine(SplitWordWithArray("baseballbase", dictArray) ?? "null"); // "baseball base"
		Console.WriteLine(SplitWordWithArray("gamebaseball", dictArray) ?? "null"); // "game baseball"
		Console.WriteLine(SplitWordWithArray("basketballgame", dictArray) ?? "null"); // null
		Console.WriteLine(SplitWordWithArray("baseballgame", dictArray) ?? "null"); // This is a corner case. Will currently split into "baseball game", but since "base ballgame" could also be valid in this instance, I wouldn't consider the case handled.
		
		// Same as above, but this time using the trie-based implementation.
		Console.WriteLine("\nTrie dictionary:");
		Node dictTrie = CreateDictTrie(dictArray);
		Console.WriteLine(SplitWordWithTrie("baseballbase", dictTrie) ?? "null"); // "baseball base"
		Console.WriteLine(SplitWordWithTrie("gamebaseball", dictTrie) ?? "null"); // "game baseball"
		Console.WriteLine(SplitWordWithTrie("basketballgame", dictTrie) ?? "null"); // null
		Console.WriteLine(SplitWordWithTrie("baseballgame", dictTrie) ?? "null"); // This is a corner case. Will currently split into "baseball game", but since "base ballgame" could also be valid in this instance, I wouldn't consider the case handled.
	}
	
	// Create the trie to use for spell checking, from an array of valid words.
	private static Node CreateDictTrie(string[] dictArray) {
		// The root node.
		Node root = new Node();
		// For every word in the dictionary, go through all the character in the word.
		for (int i = 0; i < dictArray.Length; i++) {
			string word = dictArray[i];
			// Set the current working node as the currently empty root node of the trie.
			Node node = root;
			// Set the current node to the node with the key corresponding to the character in the word.
			// If no node with this key exists, create a new one and add it under this key.
			for (int j = 0; j < word.Length; j++) {
				char character = word[j];
				if (!node.ContainsKey(character)) {
					node[character] = new Node();
				}
				node = node[character];
			}
			// Once the end of the word has been reached, set the last node as the end of the word.
			node.IsWord = true;
			
		}
		return root;
	}
	
	// Below you'll find two implementations of the word splitter. They both do the same thing,
	// but one of them uses an array as the dictionary while the other one uses a trie.
	// The methods will split an input string into multiple space-seperated words from a dictionary.
	// If the input word is in the dictionary return it.
	// If the input word doesn't contain any dictionary words, return null.
	// So "applepieplate" will split into "apple pie plate".
	
	// This implementation uses a trie as a dictionary.
	private static string SplitWordWithTrie(string word, Node dictTrie) {
		word = word.ToLower();
		// If the complete input string is already a word, just return it without splitting it.
		if (dictTrie.ContainsWord(word)) {
			return word;
		}
		// Set the current working node to the root of the dictionary trie.
		Node node = dictTrie;
		string output = null;
		for (int i = 0; i < word.Length; i++) {
			char character = word[i];
			// If the character isn't in the current node's keys, the word isn't in the dictionary.
			if (!node.ContainsKey(character)) {
				break;
			}
			// Set the current working node to the child node with the current character as the key.
			node = node[character];
			if (node.IsWord) {
				// Get the part of the string that has been determined to be a word.
				string firstPart = word.Substring(0, i + 1);
				// Then take the rest of the input string and run it through this method as the input string.
				// This way the secondPart variable will either end up being null, meaning that the rest
				// of the string isn't a valid word, or the space-seperated version of the input word.
				string secondPart = SplitWordWithTrie(word.Substring(i + 1), dictTrie);
				if (secondPart != null) {
					// Both parts are valid and can be set as candidates for the final output.
					// The reason the output is not just returned here, is because we would rather return compound 
					// words, if any exists. So instead of returning "base ball base", we return "baseball base".
					output = firstPart + " " + secondPart;
				}
			}
		}
		return output;
	}
	
	
	// This implementation uses an array as a dictionary.
	private static string SplitWordWithArray(string word, string[] dictList) {
		word = word.ToLower();
		// If the complete input string is already a word, just return it without splitting it.
		if (dictList.Contains(word)) {
			return word;
		}
		string output = null;
		for (int i = 1; i < word.Length; i++) {
			// Get the fist i characters of the string and check if they're a valid word.
			string firstPart = word.Substring(0, i);
			if (dictList.Contains(firstPart)) {
				// Then take the rest of the input string and run it through this method as the input string.
				// This way the secondPart variable will either end up being null, meaning that the rest
				// of the string isn't a valid word, or the space-seperated version of the input word.
				string secondPart = SplitWordWithArray(word.Substring(i), dictList);
				if (secondPart != null) {
					// Both parts are valid and can be set as candidates for the final output.
					// The reason the output is not just returned here, is because we would rather return compound 
					// words, if any exists. So instead of returning "base ball base", we return "baseball base".
					output = firstPart + " " + secondPart;
				}
	  		}
		}
		return output;
	}
}

// Node class used for the trie structure.
public class Node : Dictionary<char, Node> {
	// Whether the string terminating at this node is a valid word.
	public bool IsWord { get; set; }
	// Check whether a word is contained within the node's children.
	public bool ContainsWord(string word) {
		word = word.ToLower();
		Node node = this;
		for (int i = 0; i < word.Length; i++) {
			char character = word[i];
			if (!node.ContainsKey(character)) {
				return false;
			}
			node = node[character];
		}
		return node.IsWord;
	}
}
