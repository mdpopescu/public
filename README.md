Public repository - CC-by-SA 3.0
======

* Anagrams: generates anagrams for a phrase; using a unix words dictionary (see the about box for source)

* AppendOnly: project for append-only files with variable-length records; I intend to use it in a CQRS project later on. See article at http://mdpopescu.blogspot.com/2013/09/append-only-files.html

* BinarySearchTimings: comparing different search algorithms; see article at http://mdpopescu.blogspot.com/2013/05/optimization-implementing-binary-search.html

* BruteForcePuzzle: solve puzzles of the form RAI + IAR + IA = 824 (the result must be a number); uses the evaluator from my book

* CompareExcelFiles: compares two Excel files with the same structure, based on the indicated columns; shows for each file what records are present in that one but not the other

* CompareExcelFiles2: the same project as above but started from scratch using TDD; article at http://mdpopescu.blogspot.com/2013/06/comparing-excel-files-take-two.html

* Failover: fail-over algorithm; see article at http://mdpopescu.blogspot.com/2013/06/fail-over-algorithm.html

* FSM: quick implementation of a generic finite state machine; see article at http://mdpopescu.blogspot.com/2013/08/a-generic-implementation-of-finite.html

* Hot Folder Windows Service: Windows service to monitor a specific folder and (for now) log the create / delete / change / rename events

* Inventory: (INCOMPLETE) inventory management system - mainly to use SignalR and Knockout.js

* PageFaults: project from https://www.freelancer.com/projects/C-Sharp-Programming/FIFO-OPTIMAL-LRU.html (I included the detailed spec in the project); apparently someone else is taking the same exam - https://www.freelancer.com/projects/C-Sharp-Programming/lab-rep-project-repost.4718106.html

* QRMaker: the same guy with the Yelp project had another one, a bulk QR generator; unfortunately I didn't get this job either - https://www.odesk.com/jobs/URGENT-Really-fast-Bulk-maker-zip_~01368c1f8e16aa230d

* RecursiveCompare: recursive comparison of two objects' properties (if some of those properties are objects, compare those using the same algorithm); incomplete

* SafeRedir: time-limited URL redirection; the shortened link will redirect to the real one for only a limited time (default 5 minutes) and then redirect to a "safe" link afterwards; deployed at http://red.renfieldsoftware.com/

* SimpleViewEngine: a view engine with only two directives, {{if}} / {{else}} / {{endif}} and {{foreach}} / {{endfor}}. Evaluates expressions with {{name}} and the current item itself in a foreach is {{}}.

* Sudoku: a Sudoku solver based on Peter Norvig's article at http://norvig.com/sudoku.html

* TextGeneration: (for now) using Markov chains to generate plausibly-looking texts.

* VideoSpinner: Solution for https://www.odesk.com/applications/247465623 which I did not get (I will admit that it took me far too long). However, the project was quite interesting so I wanted to finish it. I do NOT like the resulting design of the code - I was too busy trying to make it work to ensure a proper design, unfortunately. Like they say - make it work, make it good, make it fast; this is only at the "make it work" stage.

* VinReader: code to read a VIN barcode; written for https://www.guru.com/jobs/.NET-VIN-Barcode-Application/978230

* YelpSearch: search Yelp for businesses (eg hotels) near a given zip; solution for https://www.odesk.com/jobs/~~d5624935369d26d1 (I didn't get the job so the code is mine)
