__author__ = 'marcel'

import unittest

from Users import Users

class testUsers(unittest.TestCase):

    def setUp(self):
        self.sut = Users()

    def test_add_creates_new_user(self):
        self.sut.add("Marcel", "Gigi")

        self.assertEquals(1, len(self.sut.dict))
        self.assertEquals("Marcel", list(self.sut.dict.keys())[0])

    def test_add_appends_new_user_to_list_of_followed_users(self):
        self.sut.add("Marcel", "Gigi")

        followed = self.sut.dict["Marcel"]
        self.assertEquals(1, len(followed))
        self.assertEquals("Gigi", followed[0])

    def test_add_appends_multiple_followed_users(self):
        self.sut.dict["Marcel"] = ["Gigi"]

        self.sut.add("Marcel", "Bogdan")

        followed = self.sut.dict["Marcel"]
        self.assertEquals(2, len(followed))
        self.assertEquals("Gigi", followed[0])
        self.assertEquals("Bogdan", followed[1])

    def test_add_does_not_append_same_followed_user_multiple_times(self):
        self.sut.dict["Marcel"] = ["Gigi"]

        self.sut.add("Marcel", "Gigi")

        followed = self.sut.dict["Marcel"]
        self.assertEquals(1, len(followed))

    def test_get_returns_empty_list(self):
        result = self.sut.get("Marcel")

        self.assertEquals(0, len(result))

    def test_get_returns_list_of_followed_users(self):
        list = ["Gigi", "Gogu"]
        self.sut.dict["Marcel"] = list

        result = self.sut.get("Marcel")

        self.assertEquals(list, result)
