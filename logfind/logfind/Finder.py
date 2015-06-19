class Finder:

    def __init__(self, parser, fileSystem, matcher):
        self.parser = parser
        self.fileSystem = fileSystem
        self.matcher = matcher

    def find(self, words, options=""):
        wlist = self.parser.parse_words(words)
        op = self.parser.parse_options(options)

        pattern = self.fileSystem.load(".logfind")
        file_list = self.fileSystem.get_files(pattern)
        for fileName in file_list:
            contents = self.fileSystem.load(fileName)
            self.matcher.match(contents, wlist, op)

        return []
