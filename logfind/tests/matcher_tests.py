from nose.tools import *
from logfind.Matcher import Matcher
from logfind.Logical import Logical

class matcher_tests:

    def __init__(self):
        self.sut = Matcher()

    def test_match_returns_true_if_all_words_are_found_in_contents(self):
        result = self.sut.match("Lorem ipsum dolor", ["Lorem", "ipsum"], Logical.And)

        assert_equal(True, result)

    def test_match_returns_false_if_some_words_are_not_found_in_contents(self):
        result = self.sut.match("Lorem dolor", ["Lorem", "ipsum"], Logical.And)

        assert_equal(False, result)
