from nose.tools import *
from logfind.Parser import Parser

class parser_tests:

    def __init__(self):
        self.sut = Parser()

    def test_parse_words_returns_word_list(self):
        result = self.sut.parse_words("Lorem ipsum")

        assert_equal(2, len(result))
        assert_equal("Lorem", result[0])
        assert_equal("ipsum", result[1])
