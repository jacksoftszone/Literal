// Copyright 2014 #jacksoftszone
// Licensed under GPLv3
// Refer to the LICENSE.txt file included.

namespace Literal {
    class Debug {
        public static void Log(string str) {
#if DEBUG
            System.ConsoleColor old = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.White;
            System.Console.WriteLine(str);
            System.Console.ForegroundColor = old;
#endif
        }

        public static void Error(string str) {
#if DEBUG
            System.ConsoleColor old = System.Console.ForegroundColor;
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine(str);
            System.Console.ForegroundColor = old;
#endif
        }
    }

}
