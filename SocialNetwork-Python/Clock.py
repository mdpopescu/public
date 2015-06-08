__author__ = 'marcel'

import datetime

class Clock:
    @staticmethod
    def now():
        return datetime.datetime.now()

    @staticmethod
    def set_now(value):
        Clock.now = staticmethod(lambda: value)
