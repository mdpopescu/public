Public repository - CC-by-SA 3.0
======

* Accounting: (very rudimentary) personal accounting

* Acta: a journaling database

* Anagrams: generates anagrams for a phrase; using a unix words dictionary (see the about box for source)

* AppendOnly: project for append-only files with variable-length records; I intend to use it in a CQRS project later on. See article at http://mdpopescu.blogspot.com/2013/09/append-only-files.html

* AppUpdater: component for updating an application

* BigDataProcessing: utility that processes GB-sized data sources efficiently, using Rx.NET

* BinarySearchTimings: comparing different search algorithms; see article at http://mdpopescu.blogspot.com/2013/05/optimization-implementing-binary-search.html

* BruteForcePuzzle: solve puzzles of the form RAI + IAR + IA = 824 (the result must be a number); uses the evaluator from my book

* CachingAlgorithms: comparing several caching algorithms (least recently used, most recently used and so on)

* CompareExcelFiles: compares two Excel files with the same structure, based on the indicated columns; shows for each file what records are present in that one but not the other

* CompareExcelFiles2: the same project as above but started from scratch using TDD; article at http://mdpopescu.blogspot.com/2013/06/comparing-excel-files-take-two.html

* CQRS, CQRS2: playing with ideas for implementing the CQRS pattern

* CustomCrypto: a custom encryption / decryption algorithm (NOT recommended, I am not a professional cryptographer!)

* DesktopClock: a simple desktop wallpaper clock - trying to learn WPF

* Elomen: notification service (application that can receive natural-language commands and send notification of future events)

* EverythingIsADatabase: trying to play with ideas from http://witheve.com/

* Failover: fail-over algorithm; see article at http://mdpopescu.blogspot.com/2013/06/fail-over-algorithm.html

* FindDuplicates: finds the identical / similar images in the given folder

* FSM: quick implementation of a generic finite state machine; see article at http://mdpopescu.blogspot.com/2013/08/a-generic-implementation-of-finite.html

* Giles: social assistant for managing posts to blogs / Twitter / Facebook

* GilesBot: virtual assistant / Skype bot

* Hot Folder Windows Service: Windows service to monitor a specific folder and (for now) log the create / delete / change / rename events

* httpd: Simple web server (only static files)

* Inventory: (INCOMPLETE) inventory management system - mainly to use SignalR and Knockout.js

* ISBN: Program for converting a product code (12 digits) to an ISBN number (10 digits)

* Logfind: Implements http://projectsthehardway.com/2015/06/16/project-1-logfind-2/

* MaxRecursion: answer to a problem posed by Karl Seguin on Twitter https://twitter.com/karlseguin/status/521592595282006016 - Write code that prints out the maximum depth of recursion allowed.

* PageFaults: project from https://www.freelancer.com/projects/C-Sharp-Programming/FIFO-OPTIMAL-LRU.html (I included the detailed spec in the project); apparently someone else is taking the same exam - https://www.freelancer.com/projects/C-Sharp-Programming/lab-rep-project-repost.4718106.html

* Propagators: code based on ideas from https://www.youtube.com/watch?v=O3tVctB_VSU and http://mcdonnell.mit.edu/sussman_slides.pdf

* QRMaker: the same guy with the Yelp project had another one, a bulk QR generator; unfortunately I didn't get this job either - https://www.odesk.com/jobs/URGENT-Really-fast-Bulk-maker-zip_~01368c1f8e16aa230d

* RecursiveCompare: recursive comparison of two objects' properties (if some of those properties are objects, compare those using the same algorithm); incomplete

* RxProblem: how to implement the pipeline pattern using Rx, allowing an entire stream to be processed even if some of the filters in the pipeline throw exceptions

* SafeRedir: time-limited URL redirection; the shortened link will redirect to the real one for only a limited time (default 5 minutes) and then redirect to a "safe" link afterwards; deployed at http://red.renfieldsoftware.com/

* SaveWebsiteCopy: solution for https://www.upwork.com/jobs/~01dea9d9c9418bb4f5 - the description of the project is inside, in the notes.txt file

* SimpleViewEngine: a view engine with only two directives, {{if}} / {{else}} / {{endif}} and {{foreach}} / {{endfor}}. Evaluates expressions with {{name}} and the current item itself in a foreach is {{}}.

* SlotMachine: a slot machine

* Snake: a snake game written in an event-driven fashion

* SocialClone: a FB-like site

* SocialNetwork: what the name says; written for a job interview.

* SocialNetwork-Python: same thing in Python.

* Spiral: generate a spiral with numbers from 1 to n^2, starting in the middle and going outwards

* SqlBenchmark: a benchmark written after reading https://ayende.com/blog/174273/how-to-waste-cpu-and-kill-your-disk-by-scaling-100-million-inefficiently (the idea that GUIDs as primary keys are horribly inefficient). On my computer, I INSERT 200K records/sec with INT Ids and 100K records/sec with GUIDs.

* Sudoku: a Sudoku solver based on Peter Norvig's article at http://norvig.com/sudoku.html

* SyncMaster: application for keeping folders synchronized, with some servers designated as "pull-only" or "push-only"

* TaskSpikes: I need a long-running, cancelable task that might need to interact with the UI

* TextGeneration: (for now) using Markov chains to generate plausibly-looking texts.

* TransformyClone: library based on the idea from http://www.transformy.io/

* TweetNicer: a WinForms replacement for TweetDeck with some additional features (work in progress)

* VideoSpinner: (INCOMPLETE) Solution for https://www.odesk.com/applications/247465623 which I did not get (I will admit that it took me far too long). However, the project was quite interesting so I wanted to finish it. I do NOT like the resulting design of the code - I was too busy trying to make it work to ensure a proper design, unfortunately. Like they say - make it work, make it good, make it fast; this is only at the "make it work" stage.

* VinReader: code to read a VIN barcode; written for https://www.guru.com/jobs/.NET-VIN-Barcode-Application/978230

* Virtual Machine Interpreter: implements the VM described at http://courses.cms.caltech.edu/cs11/material/c/mike/lab8/lab8.html

* VM: implements a custom virtual machine - I was bored :)

* WebScraping: a new language from retrieving data from the web plus a transpiler from that language to C#

* WebStore: Another implementation of an inventory management system, using an event store

* YelpSearch: search Yelp for businesses (eg hotels) near a given zip; solution for https://www.odesk.com/jobs/~~d5624935369d26d1 (I didn't get the job so the code is mine)
