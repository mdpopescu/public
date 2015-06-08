__author__ = 'marcel'

import unittest
import mock

import datetime

from Storage import Storage
from Formatter import Formatter
from Message import Message
from Clock import Clock
from App import App

class testApp(unittest.TestCase):

    def setUp(self):
        self.storage = Storage()
        self.sut = App(self.storage, Formatter())

    def test_post_stores_message(self):
        self.storage.add = mock.MagicMock()
        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 4, 5))

        self.sut.post("Marcel", "abcd")

        message = Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 5))
        self.storage.add.assert_called_once_with(message)

    def test_get_returns_message(self):
        self.storage.get = mock.MagicMock()
        self.storage.get.return_value = [ Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 5)) ]
        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 4, 8))

        lines = self.sut.get("Marcel")

        self.assertEquals(1, len(lines))
        self.assertEquals("abcd (3 seconds ago)", lines[0])

    def test_get_returns_messages_in_reverse_order(self):
        self.storage.get = mock.MagicMock()
        self.storage.get.return_value = [
            Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 0)),
            Message("Marcel", "efgh", datetime.datetime(2000, 1, 2, 3, 4, 2))
        ]
        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 4, 3))

        lines = self.sut.get("Marcel")

        self.assertEquals(2, len(lines))
        self.assertEquals("efgh (1 second ago)", lines[0])
        self.assertEquals("abcd (3 seconds ago)", lines[1])

    def test_get_only_returns_messages_from_given_user(self):
        self.storage.get = mock.MagicMock()
        self.storage.get.return_value = [
            Message("Marcel", "abcd", datetime.datetime(2000, 1, 2, 3, 4, 0)),
            Message("Gigi", "efgh", datetime.datetime(2000, 1, 2, 3, 4, 2))
        ]
        Clock.now = staticmethod(lambda: datetime.datetime(2000, 1, 2, 3, 4, 5))

        lines = self.sut.get("Marcel")

        self.assertEquals(1, len(lines))
        self.assertEquals("abcd (5 seconds ago)", lines[0])
