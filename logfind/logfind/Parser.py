from logfind.Logical import Logical

class Parser:

    def __init__(self):
        """ initialization """

    def parse_words(self, words):
        return words.split()

    def parse_options(self, options):
        if options == "-o":
            return Logical.Or
        else:
            return Logical.And
