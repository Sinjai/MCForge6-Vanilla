using MCForge.API.Events;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils;
namespace MCForge.Commands {
    public class CmdTest : ICommand {
        string _Name = "test";
        public string Name { get { return _Name; } }

        CommandTypes _Type = CommandTypes.Misc;
        public CommandTypes Type { get { return _Type; } }

        string _Author = "Merlin33069";
        public string Author { get { return _Author; } }

        int _Version = 1;
        public int Version { get { return _Version; } }

        string _CUD = "";
        public string CUD { get { return _CUD; } }

        byte _Permission = 0;
        public byte Permission { get { return _Permission; } }

        string[] CommandStrings = new string[1] { "test" };

        public void Use(Player p,string[] args) {
            System.Random r = new System.Random();
            string msg = "";
            while (p.IsLoggedIn) {
                Packet pp = new Packet();
                pp.Add((byte)5);
                ushort x = (ushort)r.Next(ushort.MaxValue), z = (ushort)r.Next(ushort.MaxValue), y = (ushort)r.Next(ushort.MaxValue);
                ushort a = (ushort)r.Next(ushort.MaxValue);
                pp.Add(x);
                pp.Add(z);
                pp.Add(y);
                pp.Add(a);
                p.SendPacket(pp);
                msg = "" + x + " " + z + " " + y + " " + pp.bytes[7] + " " + pp.bytes[8];
                p.SendMessage(msg);
                Logger.Log(msg);
                System.Threading.Thread.Sleep(100);
            }
            return;
            for (int i = 0; i < byte.MaxValue; i++) {
                Packet pa = new Packet();
                pa.Add((byte)5);
                //pa.Add((ushort)0); pa.Add((ushort)0); pa.Add((ushort)0);
                for (int a = 0; a < i; a++)
                    pa.Add((byte)2);
                for (int a = i; a < 255; a++) {
                    pa.Add((byte)1);
                }
                p.SendPacket(pa);
                msg = "" + i;
                p.SendMessage(msg);
                Logger.Log(msg);
                System.Threading.Thread.Sleep(500);
            }
            return;
        }

        void OnPlayerBlockChange_Normal(Player sender, BlockChangeEventArgs args) {
            sender.OnPlayerBlockChange.Normal -= OnPlayerBlockChange_Normal;
            sender.ExtraData["Datapass"] = new Vector3S(args.X, args.Z, args.Y);
            sender.OnPlayerBlockChange.Normal += new Event<Player, BlockChangeEventArgs>.EventHandler(OnPlayerBlockChange_Normal2);

        }

        void OnPlayerBlockChange_Normal2(Player sender, BlockChangeEventArgs args) {
            sender.OnPlayerBlockChange.Normal -= OnPlayerBlockChange_Normal2;
        }

        public void CallBack(Player sender, MoveEventArgs args) {
            MCForge.Utils.Logger.Log("Test: " + sender.Username + " moved!");
            sender.SendMessage("Hi!");
            sender.OnPlayerChat.All -= new ChatEvent.EventHandler(CallBack2);
        }
        public void CallBack2(Player sender, ChatEventArgs args) {
            //Logger.Log("Test: " + e.target.Username + " disconnected!");
            args.Message += "  Yeah, and Pikachu ROCKS!";
            sender.OnPlayerChat.All -= new ChatEvent.EventHandler(CallBack2);
        }

        public void Help(Player p) {

        }

        public void Initialize() {
            Command.AddReference(this, CommandStrings);
        }
    }
}
