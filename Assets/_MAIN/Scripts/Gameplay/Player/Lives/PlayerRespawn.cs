using System.Collections;
using UnityEngine;
using Cinemachine;
using System;
using MAIN_GAME;

namespace PLAYER
{
    public class PlayerRespawn : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Transform startingPoint;
        private CircleTransition circleTransition;
        private CinemachineVirtualCamera roomCamera;
        public bool isDying { get; set; } = false;
        private string levelName => GameManager.instance.LEVEL_NAME;

        void Start()
        {
            var level = LevelProgressManager.GetLevel(levelName);

            if (level.hasCheckpoint)
            {
                transform.position = level.checkpoint;
            }
            else
            {
                if (startingPoint != null)
                    transform.position = startingPoint.position;
            }
        }

        public void Initialize(Rigidbody2D rbRef, Transform start, CircleTransition transition, CinemachineVirtualCamera cam)
        {
            rb = rbRef;
            startingPoint = start;
            circleTransition = transition;
            roomCamera = cam;
        }

        public void Die(bool terminalDeath = false)
        {
            if (isDying) return;
            LevelProgressManager.AddDeathCount(GameManager.instance.LEVEL_NAME);
            StartCoroutine(Respawn(terminalDeath));
        }

        private IEnumerator Respawn(bool terminalDeath = false)
        {
            isDying = true;

            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            if (terminalDeath)
                yield return StartCoroutine(circleTransition.CloseCircle(terminalDeath));
            else
                yield return StartCoroutine(circleTransition.CloseCircle());
            transform.position = startingPoint.position;

            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            CameraManager.SwitchCamera(roomCamera);

            yield return new WaitForSeconds(1f);
            if (terminalDeath)
                yield return StartCoroutine(circleTransition.OpenCircle(terminalDeath));
            else
                yield return StartCoroutine(circleTransition.OpenCircle());

            isDying = false;
        }
    }
}
