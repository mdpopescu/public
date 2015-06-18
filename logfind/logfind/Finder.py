class Finder:

    def __init__(self, wordParser, optionsParser, fileSystem):
        self.wordParser = wordParser
        self.optionsParser = optionsParser
        self.fileSystem = fileSystem

    def find(self, words, options=""):
        self.wordParser.parse(words)
        self.optionsParser.parse(options)

        pattern = self.fileSystem.load(".logfind")
        self.fileSystem.get_files(pattern)

        return []
