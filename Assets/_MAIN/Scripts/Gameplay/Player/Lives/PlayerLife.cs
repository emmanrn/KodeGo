using System;
using UnityEngine;

namespace PLAYER
{
    public class PlayerLife : MonoBehaviour, IDamageable
    {
        private const int MAX_LIFE = 3;
        private int currentLife;
        private PlayerRespawn respawn;
        public int CurrentLives => currentLife;
        public event Action<int> OnLivesChanged;

        public void Initialize(PlayerRespawn respawn)
        {
            this.respawn = respawn;
        }

        void Start()
        {
            currentLife = MAX_LIFE;
            GeneralManager.instance.RegisterPlayer(this);
            OnLivesChanged?.Invoke(currentLife);
        }

        public void TakeDamage(int dmg)
        {
            currentLife -= dmg;
            Debug.Log(currentLife);

            OnLivesChanged?.Invoke(currentLife);



            if (currentLife <= 0)
            {
                Debug.Log("Dead Repaawn");
                respawn.Die(true);

                currentLife = MAX_LIFE;
                OnLivesChanged?.Invoke(currentLife);

                GameEvents.PlayerDied();

            }
        }



    }

}