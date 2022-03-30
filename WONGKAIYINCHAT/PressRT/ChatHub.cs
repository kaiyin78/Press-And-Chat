using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Linq;
using System.Collections.Concurrent;



namespace PressRT
{
   
    public class Player1
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        
        

        public Player1 (string id, string icon, string name) 
        {
            Id = id;
            Icon = icon;
            Name = name;
            
        }
    }
    
    public class ChatHub : Hub
    {
        private static List <Player1> players= new List <Player1>();
        private static int count=0;
        private string storeImage;
       
        // private static string typeTd;

        
        public async Task SendText(string icon, string name, string message, string id)
        {
            Player1 player = players.Find(p => p.Id == id);
            string sender = Context.ConnectionId;
            //string receiver = Context.ConnectionId;
            if (player != null)
            {
                await Clients.Caller.SendAsync("ReceiveText", icon, name, message, "caller", id);
                await Clients.Client(id).SendAsync("ReceiveText", icon, name, message, "others", sender);
                
            }
            else 
            {
                await Clients.Caller.SendAsync("ReceiveText", icon, name, message, "caller", "GLOBAL");
                await Clients.Others.SendAsync("ReceiveText", icon, name, message, "others", "GLOBAL");
            }
            
        }
        public override async Task OnConnectedAsync()
        {
           string page = Context.GetHttpContext().Request.Query["page"];

            switch (page)
            {
                case "list"     : await PlayerList();           break;
                
            }

            await base.OnConnectedAsync();
        }
        public async Task store(string dd)
        {
            storeImage = dd;
            await PlayerList();
        }

        public async Task PlayerList()
        {
            if (storeImage != null)
            {
                string id = Context.ConnectionId;
                
                string icon = storeImage;
                string name = Context.GetHttpContext().Request.Query["name"];

                Player1 p = new Player1(id, icon, name);
                players.Add(p);
                count++;
                await Clients.All.SendAsync("PlayerList", players);
                await Clients.All.SendAsync("UpdateStatus", count, $"<b>{name}</b> joined");
                // await Clients.Others.SendAsync("Usertyping", name);
            }
            else {
                // nothing 
            }
        }
        
     public async Task Usertyping()
        {
      
                string name = Context.GetHttpContext().Request.Query["name"];
                await Clients.Others.SendAsync("Usertyping", name);
            
        }
        
        public async Task SendImage(string icon, string name, string url, string id)
        {
            Player1 player = players.Find(p => p.Id == id);
            string sender = Context.ConnectionId;

            if (player != null)
            {
                await Clients.Caller.SendAsync("ReceiveImage", icon, name, url, "caller", id);
                await Clients.Client(id).SendAsync("ReceiveImage", icon, name, url, "others", sender);
                
            }
            else 
            {
                await Clients.Caller.SendAsync("ReceiveImage", icon, name, url, "caller", "GLOBAL");
                await Clients.Others.SendAsync("ReceiveImage", icon, name, url, "others", "GLOBAL");
            }

        }

        public async Task SendYouTube(string icon, string name,string youtubeid, string id)
        {
            Player1 player = players.Find(p => p.Id == id);
            string sender = Context.ConnectionId;

            if (player != null)
            {
                await Clients.Caller.SendAsync("ReceiveYoutube", icon, name, youtubeid, "caller", id);
                await Clients.Client(id).SendAsync("ReceiveYoutube", icon, name, youtubeid, "others", sender);
            }
            else 
            {
                await Clients.Others.SendAsync("ReceiveYouTube", icon, name, youtubeid, "others", "GLOBAL");
                await Clients.Caller.SendAsync("ReceiveYouTube", icon, name, youtubeid, "caller", "GLOBAL");
            }
            
        }   


        public override async Task OnDisconnectedAsync(Exception exception) 
        {
            
            string page = Context.GetHttpContext().Request.Query["page"];

            switch (page)
            {
                case "list"    : await ListDisConnected();  break;
                
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task ListDisConnected()
        {
            string id = Context.ConnectionId;

            Player1 player = players.Find(p => p.Id == id);
            players.Remove(player);
            count--;

            await Clients.All.SendAsync("PlayerList", players);
            await Clients.All.SendAsync("UpdateStatus", count, $"<b>{player.Name}</b> Left");
        }







        
    }
}
