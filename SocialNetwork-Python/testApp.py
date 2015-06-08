__author__ = 'marcel'

import unittest
import mock

import datetime

from App import App
from Storage import Storage
from Message import Message
from Clock import Clock

class testApp(unittest.TestCase):

    def setUp(self):
        self.storage = Storage()
        self.sut = App(self.storage)

    def test_post_stores_message(self):
        self.storage.add = mock.MagicMock()
        Clock.set_now(datetime.datetime(2000, 1, 2, 3, 4, 5))

        self.sut.post("Marcel", "abcd")

        message = Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 5))
        self.storage.add.assert_called_once_with(message)

    @unittest.skip("for now")
    def test_get_returns_message(self):
        self.storage.get = mock.MagicMock()
        self.storage.get.return_value = [ Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 5)) ]
        Clock.set_now(datetime.datetime(2000, 1, 2, 3, 4, 8))

        lines = self.sut.get("Marcel")

        self.assertEquals(len(lines), 1)
        self.assertEquals(lines[0], "abcd (3 seconds ago)")

    @unittest.skip("for now")
    def test_get_returns_messages_in_reverse_order(self):
        self.storage.get = mock.MagicMock()
        self.storage.get.return_value = [
            Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 0)),
            Message("Marcel", "efgh", datetime.datetime(2000, 1, 2, 3, 4, 2))
        ]
        Clock.set_now(datetime.datetime(2000, 1, 2, 3, 4, 3))

        lines = self.sut.get("Marcel")

        self.assertEquals(len(lines), 2)
        self.assertEquals(lines[0], "efgh (1 second ago)")
        self.assertEquals(lines[1], "abcd (3 seconds ago)")

def suite():
    suite = unittest.TestSuite()

    suite.addTest(testApp("test_post_stores_message"))
    suite.addTest(testApp("test_get_returns_message"))

    return suite
