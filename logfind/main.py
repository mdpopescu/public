import sys
from logfind.Finder import Finder
from logfind.Parser import Parser
from logfind.FileSystem import FileSystem
from logfind.Matcher import Matcher

def main(argv=None):
    if argv is None:
        argv = sys.argv

    options = ""
    if "-o" in argv:
        options = "-o"
        argv.remove("-o")

    finder = Finder(Parser(), FileSystem(), Matcher())

    args = " ".join(argv[1:])
    print finder.find(args, options)

if __name__ == "__main__":
    sys.exit(main())
