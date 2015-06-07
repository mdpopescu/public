__author__ = 'marcel'

import unittest
import mock

from App import App
from Storage import Storage
from Message import Message

class testApp(unittest.TestCase):

    def setUp(self):
        self.storage = Storage()
        self.sut = App(self.storage)

    def test_post_stores_message(self):
        self.storage.add = mock.MagicMock(name = "Add")

        self.sut.post("Marcel", "abcd")

        message = Message("Marcel", "abcd")
        self.storage.add.assert_called_once_with(message)

def suite():
    suite = unittest.TestSuite()

    suite.addTest(testApp("test_post_stores_message"))

    return suite
