using System;
using System.Collections.Generic;
using System.Linq;

class MainClass {
	// An array of valid words. Would normally contain way more words.
	private static string[] dictArray = new string[] {
		"all",
		"base",
		"ball",
		"baseball",
		"game",
		"ballgame" // Not a real word, used for testing.
	};
	
	public static void Main (string[] args) {
		SplitWordWithArray("baseballbase", dictArray); // "baseball base"
		SplitWordWithArray("gamebaseball", dictArray); // "game baseball"
		SplitWordWithArray("baseballgame", dictArray); // This is a corner case. Will currently split into "baseball game", but since "base ballgame" could also be valid in this instance, I wouldn't consider the case handled.
		
		Node dictTrie = CreateDictTrie(dictArray);
		SplitWordWithTrie("baseballbase", dictTrie); // "baseball base"
		SplitWordWithTrie("gamebaseball", dictTrie); // "game baseball"
		SplitWordWithTrie("baseballgame", dictTrie); // This is a corner case. Will currently split into "baseball game", but since "base ballgame" could also be valid in this instance, I wouldn't consider the case handled.
	}
	
	// Create the trie to use for spell checking, based on an array of valid words.
	private static Node CreateDictTrie(string[] dictArray) {
		Node root = new Node();
		for (int i = 0; i < dictArray.Length; i++) {
			string word = dictArray[i];
			Node node = root;
			for (int j = 0; j < word.Length; j++) {
				char character = word[j];
				if (!node.ContainsKey(character)) {
					node[character] = new Node();
				}
				node = node[character];
			}
			node.IsWord = true;
			
		}
		return root;
	}
	
	// Check whether a string is a valid word.
	private static bool IsWord(string word, Node dictTrie) {
		word = word.ToLower();
		Node node = dictTrie;
		for (int i = 0; i < word.Length; i++) {
			char character = word[i];
			if (!node.ContainsKey(character)) {
				return false;
			}
			node = node[character];
		}
		return node.IsWord;
	}
	
	// Below you'll find two implementations of the word splitter. They both do the same thing, but one of them uses an array as the dictionary while the other one uses a trie, created using the CreateDictTrie method.
	// The methods will split an input string into multiple space-seperated words from a dictionary.
	// If the input word is in the dictionary return it.
	// If the input word doesn't contain any dictionary words, return null.
	// So "applepieplate" will split into "apple pie plate".
	
	// This implementation uses a trie as a dictionary.
	private static string SplitWordWithTrie(string word, Node dictTrie) {
		word = word.ToLower();
		if (IsWord(word, dictTrie)) {
			return word;
		}
		Node node = dictTrie;
		string firstPart = null;
		string secondPart = null;
		for (int i = 0; i < word.Length; i++) {
			char character = word[i];
			if (!node.ContainsKey(character)) {
				break;
			}
			node = node[character];
			if (node.IsWord) {
				string newFirstPart = word.Substring(0, i + 1);
				string newSecondPart = SplitWordWithTrie(word.Substring(i + 1), dictTrie);
				if (newFirstPart != null && newSecondPart != null) {
					firstPart = newFirstPart;
					secondPart = newSecondPart;
				}
			}
		}
		if (firstPart != null && secondPart != null) {
			return firstPart + " " + secondPart;
		}
		return null;
	}
	
	
	// This implementation uses an array as a dictionary.
	private static string SplitWordWithArray(string word, string[] dictList) {
		word = word.ToLower();
		if (dictList.Contains(word)) {
			return word;
		}
		string firstPart = null;
		string secondPart = null;
		for (int i = 1; i < word.Length; i++) {
			string newFirstPart = word.Substring(0, i);
			if (dictList.Contains(newFirstPart)) {
				string newSecondPart = SplitWordWithArray(word.Substring(i), dictList);
				if (newSecondPart != null) {
					firstPart = newFirstPart;
					secondPart = newSecondPart;
				}
	  		}
		}
		if (firstPart != null && secondPart != null) {
			return firstPart + " " + secondPart;
		}
		return null;
	}
}

// Class used for the trie structure.
public class Node : Dictionary<char, Node> {
	public bool IsWord { get; set; }
}
