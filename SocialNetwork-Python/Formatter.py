__author__ = 'marcel'

class Formatter:
    def Format(self, duration):
        if duration.seconds == 1:
            return "1 second";

        return str(duration.seconds) + " seconds"
