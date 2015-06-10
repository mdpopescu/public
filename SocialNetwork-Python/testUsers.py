__author__ = 'marcel'

import unittest
import mock

from Users import Users

class testUsers(unittest.TestCase):

    def setUp(self):
        self.sut = Users()

    def test_add_creates_new_user(self):
        self.sut.add("Marcel", "Gigi")

        self.assertEquals(1, len(self.sut.dict))
        self.assertEquals("Marcel", list(self.sut.dict.keys())[0])
