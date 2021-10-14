using System;
using System.Collections.Generic;
using System.Text;

namespace BambooNetCode
{
    public class GroupsManager
    {
        private static Dictionary<int,Group> groupsList_;
        
        public static Dictionary<int, Group> groupsList { get { return groupsList_; } }
    
        public static void InitializeGroupManager()
        {
            groupsList_ = new Dictionary<int, Group>();

            for (int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                groupsList_.Add(i, new Group(i));
            }
        }
        public static void JoinEmptyGroup(Client _client)
        {
            for (int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                if (groupsList[i].isEmpty)
                {
                    groupsList_[i].JointGroup(_client);
                    return;
                }
            }
        }
        public static void JoinAnyGroup(Client _client)
        {
            for(int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                if (!groupsList_[i].isFull && groupsList[i].isOpen)
                {
                    groupsList_[i].JointGroup(_client);
                    return;
                }
            }
            // TODO : send all groups are full request
        }
        public static void JoinClientGroupWithID(int _clientIDJoint, Client _client)
        {
            for (int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                foreach (int x in groupsList[i].clientsID)
                {
                    if (x == _clientIDJoint)
                    {
                        if (groupsList[i].isOpen)
                        {
                            if (groupsList[i].matchMaking == null)
                            {
                                groupsList[i].JointGroup(_client);
                            }else
                            {
                                //TODO : send "Player is already in game" request;
                            }
                            return;
                        }
                        //TODO : send "group is not open" request
                    }
                }
            }

        }
        public static void JoinClientGroupWithUsername(string _clientUsername, Client _client)
        {
            for (int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                foreach (int x in groupsList[i].clientsID)
                {
                    if (Server.clientList[x].username == _clientUsername)
                    {
                        if (groupsList[i].isOpen)
                        {
                            if (groupsList[i].matchMaking == null)
                            {
                                groupsList[i].JointGroup(_client);
                            }
                            else
                            {
                                //TODO : send "Player is already in game" request;
                            }
                            return;
                        }
                        //TODO : send "group is not open" request
                    }
                }
            }

        }
        public static void JoinClientGroupWithSerialCode(string _serialCode, Client _client)
        {
            for (int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                if(groupsList[i].serialCode == _serialCode)
                {
                    if (groupsList[i].matchMaking == null)
                    {
                        groupsList[i].JointGroup(_client);
                    }
                    else
                    {
                        //TODO : send "Player is already in game" request;
                    }
                    return;
                }
            }
        }
        public static void ClientDisconnect(Client _client)
        {
            for (int i = 1; i <= Constants.SERVER_PLAYER_MAX; i++)
            {
                foreach (int x in groupsList[i].clientsID)
                {
                    if (x == _client.id)
                    {
                        groupsList[i].LeaveGroup(_client);
                        return;
                    }
                }
            }
        }
    }
}
