class Matcher:

    def __init__(self):
        """ initialization """

    def match(self, contents, words, operation):
        contains = [word for word in words if word in contents]
        return len(contains) == len(words)
