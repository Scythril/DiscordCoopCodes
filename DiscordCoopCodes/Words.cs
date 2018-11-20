using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordCoopCodes {
    public class Words {
        private Random _rnd;
        private List<String> _wordList;

        public Words() {
            _rnd = new Random();
            _wordList = WordList;
        }

        public string GetRandomWord() {
            return FirstCharToUpper(WordList[_rnd.Next(WordList.Count)]);
        }

        public string GetRandomNumber() {
            return _rnd.Next(99).ToString();
        }

        public string FirstCharToUpper(string input) {
            switch (input) {
                case null:
                case "":
                    return input;
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        private List<String> WordList {
            get {
                return new List<String> {
                    "table",
                    "hate",
                    "handsome",
                    "intend",
                    "head",
                    "repeat",
                    "billowy",
                    "whistle",
                    "cheese",
                    "roof",
                    "awful",
                    "nippy",
                    "need",
                    "loss",
                    "splendid",
                    "disarm",
                    "pail",
                    "serious",
                    "holistic",
                    "wacky",
                    "healthy",
                    "road",
                    "coast",
                    "quizzical",
                    "liquid",
                    "descriptive",
                    "sin",
                    "simplistic",
                    "need",
                    "smell",
                    "spring",
                    "defiant",
                    "abortive",
                    "battle",
                    "kittens",
                    "white",
                    "queue",
                    "eight",
                    "texture",
                    "snails",
                    "abrasive",
                    "measly",
                    "match",
                    "power",
                    "giddy",
                    "fuzzy",
                    "voyage",
                    "scatter",
                    "humorous",
                    "bump",
                    "pricey",
                    "like",
                    "add",
                    "delight",
                    "dust",
                    "vague",
                    "romantic",
                    "succeed",
                    "weigh",
                    "governor",
                    "venomous",
                    "enjoy",
                    "tug",
                    "breezy",
                    "question",
                    "structure",
                    "wash",
                    "uppity",
                    "redundant",
                    "puncture",
                    "public",
                    "share",
                    "drain",
                    "question",
                    "dinosaurs",
                    "hands",
                    "elfin",
                    "utter",
                    "scream",
                    "cow",
                    "float",
                    "cakes",
                    "sticks",
                    "sweater",
                    "dam",
                    "sidewalk",
                    "oil",
                    "rock",
                    "examine",
                    "agonizing",
                    "list",
                    "vegetable",
                    "thread",
                    "flock",
                    "abashed",
                    "irritate",
                    "knife",
                    "stew",
                    "live",
                    "afterthought"
                };
            }
        }
    }

}
