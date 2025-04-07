using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Memory_Game.Model
{
    public class GameState
    {
        public string Username { get; set; }
        public string Category { get; set; }
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public DateTime SavedAt { get; set; }
        public bool IsTimerPaused { get; set; }
        public bool IsPaused { get; set; }
        public int Moves { get; set; }
        public int Matches { get; set; }
        public List<CardState> Cards { get; set; }

        public GameState()
        {
            Cards = new List<CardState>();
            SavedAt = DateTime.Now;
        }
    }

    public class CardState
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Position { get; set; }
        public int CardId { get; set; }
    }
}