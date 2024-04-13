using UnityEngine;
using System.Collections;

namespace app_system
{
    /// <summary>
    /// 現在再生中のアニメーションの再生終了待ちをするためのカスタムコルーチン
    /// </summary>
    public class WaitForAnimation : CustomYieldInstruction
    {
        private Animator _animator;
        private int _lastStateHash = 0;
        private int _layerNo = 0;


        public static System.Collections.IEnumerator Wait(Animator animator)
        {
            yield return new app_system.WaitForAnimation(animator);
        }

        public WaitForAnimation(Animator animator, int layerNo = 0)
        {
            _layerNo = layerNo;
            _animator = animator;
            _lastStateHash = animator.GetCurrentAnimatorStateInfo(layerNo).fullPathHash;

        }

        public override bool keepWaiting
        {
            get
            {
                if (_animator == null)
                {
                    return false;
                }
                // 現在のアニメーションステート
                var currentAnimatorState = _animator.GetCurrentAnimatorStateInfo(_layerNo);

                // 再生が終わったらfalse
                return currentAnimatorState.fullPathHash == _lastStateHash && (currentAnimatorState.normalizedTime < 1);
            }
        }
    }
} // app_system
