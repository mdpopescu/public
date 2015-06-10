__author__ = 'marcel'

class Users:

    def __init__(self):
        self.dict = {}

    def add(self, user, other):
        self.dict[user] = []

    def get(self, user):
        " do something "