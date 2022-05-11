using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Server
{
    public class RoleGiver : MonoBehaviour
    {
        public enum Roles
        {
            Hider = 0, 
            Seeker = 1
        }
        
        private readonly List<int> _givenRoles = new List<int>();

        // Give the role to the player's object.
        // There is two roles - Hider and Seeker.
        // Hider role equals to 0 and Seeker role equals to 1.
        public int GiveRole(int playersInRoom)
        {
            int role = Random.Range(0, 2);

            // If we've got hider role.
            if (role == 0)
            {
                // If it's last role to give, but we still don't have seeker we should give the seeker role.
                if (!_givenRoles.Contains(1) && _givenRoles.Count >= playersInRoom - 1)
                {
                    _givenRoles.Add(1);
                    return 1;
                }
                else
                {
                    _givenRoles.Add(0);
                    return 0;
                }
            }
            // If we've got seeker role.
            else if (role == 1)
            {
                // If we already gave seeker to someone we need to give hider role to this player.
                if (_givenRoles.Contains(role))
                {
                    _givenRoles.Add(0);
                    return 0;
                }
                else
                {
                    _givenRoles.Add(1);
                    return 1;
                }
            }

            return -1;
        }
    }
}
