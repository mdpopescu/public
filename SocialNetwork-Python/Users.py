__author__ = 'marcel'

class Users:

    def __init__(self):
        self.dict = {}

    def add(self, user, other):
        list = self.get(user)
        if not (other in list):
            list.append(other)
        self.dict[user] = list

    def get(self, user):
        return self.dict.get(user) or []
