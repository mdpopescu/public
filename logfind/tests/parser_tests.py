from nose.tools import *
from logfind.Parser import Parser
from logfind.Logical import Logical

class parser_tests:

    def __init__(self):
        self.sut = Parser()

    def test_parse_words_returns_word_list(self):
        result = self.sut.parse_words("Lorem ipsum")

        assert_equal(2, len(result))
        assert_equal("Lorem", result[0])
        assert_equal("ipsum", result[1])

    def test_parse_options_returns_logical_and_for_empty_string(self):
        result = self.sut.parse_options("")

        assert_equal(Logical.And, result)

    def test_parse_options_returns_logical_or_for_dash_o(self):
        result = self.sut.parse_options("-o")

        assert_equal(Logical.Or, result)

    def test_parse_options_returns_logical_and_for_any_other_value(self):
        result = self.sut.parse_options("xyz")

        assert_equal(Logical.And, result)
