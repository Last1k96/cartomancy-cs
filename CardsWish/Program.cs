using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CardsWish
{
    public static class Utils
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                } while (!(box[0] < n * (byte.MaxValue / n)));

                var k = box[0] % n;
                n--;

                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    internal class Program
    {
        public static List<Card> MakeDeck()
        {
            var deck = new List<Card>();


            for (var d = 0; d < 4; d++)
            for (var i = 0; i < 9; i++)
                deck.Add(new Card(i, d));

            deck.Shuffle();

            return deck;
        }


        public static List<List<Card>> SplitToN(List<Card> deck, int n)
        {
            var decks = new List<List<Card>>();
            for (var i = 0; i < n; i++) decks.Add(new List<Card>());

            for (var i = 0; i < deck.Count; i++)
                decks[i % n].Add(deck[i]);

            foreach (var d in decks) d.Reverse();
            return decks;
        }

        public static void SplitAce(List<Card> deck, out List<Card> left, out List<Card> right)
        {
            var aceIndex = deck.FindIndex(c => c.Val == "Т");
            if (aceIndex == -1)
            {
                left = deck;
                right = new List<Card>();
            }
            else
            {
                left = deck.GetRange(0, aceIndex);
                right = deck.GetRange(aceIndex, deck.Count - aceIndex);
            }
        }


        public static void Print(List<Card> deck)
        {
            foreach (var card in deck)
            {
                Console.ForegroundColor = card.Color;
                Console.BackgroundColor = ConsoleColor.White;

                Console.Write(card);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintLine(List<Card> deck)
        {
            Print(deck);
            Console.WriteLine();
        }


        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                var deck = MakeDeck();
                Console.Clear();
                Console.WriteLine("Перемешали колоду: ");
                PrintLine(deck);

                for (var i = 4; i >= 1; i--)
                {
                    var decks = SplitToN(deck, i);
                    deck.Clear();

                    Console.WriteLine("\nДелим на " + i + " кучи:");
                    foreach (var subdeck in decks)
                    {
                        SplitAce(subdeck, out var left, out var right);
                        Print(left);
                        Console.Write("-- ");
                        PrintLine(right);
                        deck.InsertRange(0, right);
                    }

                    Console.Write("Собираем в одну: ");
                    PrintLine(deck);
                }

                Console.WriteLine();
                PrintLine(deck);
                Console.WriteLine(deck.Count == 4 ? "Твое желание сбудется!" : "В следующий раз обязательно повезет.");
                Console.WriteLine("Еще раз?");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    return;
            }
        }

        public struct Card
        {
            private static readonly ConsoleColor[] Colors =
                {ConsoleColor.Red, ConsoleColor.Black, ConsoleColor.Red, ConsoleColor.Black};

            private static readonly string[] Symbols = {"♥", "♠", "♦", "♣"};
            private static readonly string[] Values = {"6", "7", "8", "9", "10", "В", "Д", "К", "Т"};

            private readonly int _val;
            private readonly int _suit;

            public string Val => Values[_val];
            public string Suit => Symbols[_suit];
            public ConsoleColor Color => Colors[_suit];

            public Card(int val, int suit)
            {
                _val = val;
                _suit = suit;
            }

            public override string ToString()
            {
                return Val + Suit;
            }
        }
    }
}