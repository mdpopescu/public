class Finder:

    def __init__(self, wordParser, optionsParser):
        self.wordParser = wordParser
        self.optionsParser = optionsParser

    def find(self, words, options=""):
        self.wordParser.parse(words)
        self.optionsParser.parse(options)

        return []
