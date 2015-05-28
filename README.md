Public repository - CC-by-SA 3.0
======

* Anagrams: generates anagrams for a phrase; using a unix words dictionary (see the about box for source)

* AppendOnly: project for append-only files with variable-length records; I intend to use it in a CQRS project later on. See article at http://mdpopescu.blogspot.com/2013/09/append-only-files.html

* BinarySearchTimings: comparing different search algorithms; see article at http://mdpopescu.blogspot.com/2013/05/optimization-implementing-binary-search.html

* BruteForcePuzzle: solve puzzles of the form RAI + IAR + IA = 824 (the result must be a number); uses the evaluator from my book

* CompareExcelFiles: compares two Excel files with the same structure, based on the indicated columns; shows for each file what records are present in that one but not the other

* CompareExcelFiles2: the same project as above but started from scratch using TDD; article at http://mdpopescu.blogspot.com/2013/06/comparing-excel-files-take-two.html

* CQRS, CQRS2: playing with ideas for implementing the CQRS pattern

* DesktopClock: a simple desktop wallpaper clock - trying to learn WPF

* Failover: fail-over algorithm; see article at http://mdpopescu.blogspot.com/2013/06/fail-over-algorithm.html

* FindDuplicates: finds the identical / similar images in the given folder

* FSM: quick implementation of a generic finite state machine; see article at http://mdpopescu.blogspot.com/2013/08/a-generic-implementation-of-finite.html

* Hot Folder Windows Service: Windows service to monitor a specific folder and (for now) log the create / delete / change / rename events

* httpd: Simple web server (only static files)

* Inventory: (INCOMPLETE) inventory management system - mainly to use SignalR and Knockout.js

* ISBN: Program for converting a product code (12 digits) to an ISBN number (10 digits)

* MaxRecursion: answer to a problem posed by Karl Seguin on Twitter https://twitter.com/karlseguin/status/521592595282006016 - Write code that prints out the maximum depth of recursion allowed.

* PageFaults: project from https://www.freelancer.com/projects/C-Sharp-Programming/FIFO-OPTIMAL-LRU.html (I included the detailed spec in the project); apparently someone else is taking the same exam - https://www.freelancer.com/projects/C-Sharp-Programming/lab-rep-project-repost.4718106.html

* QRMaker: the same guy with the Yelp project had another one, a bulk QR generator; unfortunately I didn't get this job either - https://www.odesk.com/jobs/URGENT-Really-fast-Bulk-maker-zip_~01368c1f8e16aa230d

* RecursiveCompare: recursive comparison of two objects' properties (if some of those properties are objects, compare those using the same algorithm); incomplete

* RxProblem: how to implement the pipeline pattern using Rx, allowing an entire stream to be processed even if some of the filters in the pipeline throw exceptions

* SafeRedir: time-limited URL redirection; the shortened link will redirect to the real one for only a limited time (default 5 minutes) and then redirect to a "safe" link afterwards; deployed at http://red.renfieldsoftware.com/

* SimpleViewEngine: a view engine with only two directives, {{if}} / {{else}} / {{endif}} and {{foreach}} / {{endfor}}. Evaluates expressions with {{name}} and the current item itself in a foreach is {{}}.

* SocialNetwork: what the name says; written for a job interview.

* Sudoku: a Sudoku solver based on Peter Norvig's article at http://norvig.com/sudoku.html

* TaskSpikes: I need a long-running, cancelable task that might need to interact with the UI

* TextGeneration: (for now) using Markov chains to generate plausibly-looking texts.

* TransformyClone: library based on the idea from http://www.transformy.io/

* TweetNicer: a WinForms replacement for TweetDeck with some additional features (work in progress)

* VideoSpinner: (INCOMPLETE) Solution for https://www.odesk.com/applications/247465623 which I did not get (I will admit that it took me far too long). However, the project was quite interesting so I wanted to finish it. I do NOT like the resulting design of the code - I was too busy trying to make it work to ensure a proper design, unfortunately. Like they say - make it work, make it good, make it fast; this is only at the "make it work" stage.

* VinReader: code to read a VIN barcode; written for https://www.guru.com/jobs/.NET-VIN-Barcode-Application/978230

* Virtual Machine Interpreter: implements the VM described at http://courses.cms.caltech.edu/cs11/material/c/mike/lab8/lab8.html

* WebStore: Another implementation of an inventory management system, using an event store

* YelpSearch: search Yelp for businesses (eg hotels) near a given zip; solution for https://www.odesk.com/jobs/~~d5624935369d26d1 (I didn't get the job so the code is mine)
