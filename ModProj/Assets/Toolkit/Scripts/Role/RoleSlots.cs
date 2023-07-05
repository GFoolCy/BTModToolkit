#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CrossLink
{
    public class RoleSlots : MonoBehaviour
    {
        public GameObject handSlotLeft;
        public GameObject handSlotRight;
        public GameObject shoulderSlotLeft;
        public GameObject shoulderSlotRight;

        [EasyButtons.Button]
        public void AutoGetHandTransform()
        {
            var animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Please ensure that the fbxPrefab has an Animator " +
                    "and that the AnimationType option in the fbx file is Humanoid.");
                return;
            }

            if(!handSlotLeft) {
                Transform handTransformLeft = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                handSlotLeft = new GameObject("LWeapon Point");
                handSlotLeft.transform.parent = handTransformLeft;
                handSlotLeft.transform.localPosition = new Vector3(0,0,0);
                handSlotLeft.transform.localRotation = new Quaternion(0,0,0,0);
                handSlotLeft.transform.localScale = new Vector3(1,1,1);
                handSlotLeft.transform.Rotate(new Vector3(0,90,0));

                EditorUtility.SetDirty(this);
            }

            if(!handSlotRight) {
                Transform handTransformRight = animator.GetBoneTransform(HumanBodyBones.RightHand);
                handSlotRight = new GameObject("RWeapon Point");
                handSlotRight.transform.parent = handTransformRight;
                handSlotRight.transform.localPosition = new Vector3(0,0,0);
                handSlotRight.transform.localRotation = new Quaternion(0,0,0,0);
                handSlotRight.transform.localScale = new Vector3(1,1,1);
                handSlotRight.transform.Rotate(new Vector3(0,-90,0));

                EditorUtility.SetDirty(this);
            }

            if(!shoulderSlotLeft) {
                Transform shoulderTransformLeft = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
                shoulderSlotLeft = new GameObject("LWeapon Spine");
                shoulderSlotLeft.transform.parent = shoulderTransformLeft;
                shoulderSlotLeft.transform.localPosition = new Vector3(0,0,0);
                shoulderSlotLeft.transform.localRotation = new Quaternion(0,0,0,0);
                shoulderSlotLeft.transform.localScale = new Vector3(1,1,1);
                shoulderSlotLeft.transform.Translate(new Vector3(-0.2f,0,0));

                EditorUtility.SetDirty(this);
            }

            if(!shoulderSlotRight) {
                Transform shoulderTransformRight = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
                shoulderSlotRight = new GameObject("RWeapon Spine");
                shoulderSlotRight.transform.parent = shoulderTransformRight;
                shoulderSlotRight.transform.localPosition = new Vector3(0,0,0);
                shoulderSlotRight.transform.localRotation = new Quaternion(0,0,0,0);
                shoulderSlotRight.transform.localScale = new Vector3(1,1,1);
                shoulderSlotRight.transform.Translate(new Vector3(0.2f,0,0));

                EditorUtility.SetDirty(this);
            }


            Debug.LogWarning("Please check if this is correct after use. If not, " +
                "please assign the handTrans of the handPoseControl manually.");
        }

        private void OnDrawGizmos()
        {
            var color = Color.green;
            color.a = 0.3f;
            Gizmos.color = color;
            float dist = 0.5f;

            if (handSlotLeft) {
                Gizmos.DrawWireSphere(handSlotLeft.transform.position, 0.1f);
                Vector3 direction = transform.TransformDirection(handSlotLeft.transform.forward) * dist;
                Gizmos.DrawRay(handSlotLeft.transform.position, direction);
            }

            if (handSlotRight) {
                Gizmos.DrawWireSphere(handSlotRight.transform.position, 0.1f);
                Vector3 direction = transform.TransformDirection(handSlotRight.transform.forward) * dist;
                Gizmos.DrawRay(handSlotRight.transform.position, direction);
            }

            if (shoulderSlotLeft) {
                Gizmos.DrawWireSphere(shoulderSlotLeft.transform.position, 0.1f);
                Vector3 direction = transform.TransformDirection(shoulderSlotLeft.transform.forward) * dist;
                Gizmos.DrawRay(shoulderSlotLeft.transform.position, direction);
            }

            if (shoulderSlotRight) {
                Gizmos.DrawWireSphere(shoulderSlotRight.transform.position, 0.1f);
                Vector3 direction = transform.TransformDirection(shoulderSlotRight.transform.forward) * dist;
                Gizmos.DrawRay(shoulderSlotRight.transform.position, direction);
            }
        }
        
    }
}
#endif
