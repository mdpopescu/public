__author__ = 'marcel'

class Formatter:
    def Format(self, duration):
        if duration.second == 0:
            return "0 seconds";

        return "1 second"
