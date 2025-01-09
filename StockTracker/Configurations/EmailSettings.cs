﻿namespace StockTracker.Configurations
{
    public class EmailSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Recipients { get; set; }
    }
}
