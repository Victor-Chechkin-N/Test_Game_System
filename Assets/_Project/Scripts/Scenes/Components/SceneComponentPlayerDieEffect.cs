using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Project
{
	public class SceneComponentPlayerDieEffect : MonoBehaviour
	{
		private const float MaxEdge = 1.75f;
		private const string EdgeKey = "_Edge";
		
		[SerializeField]
		private Material grayscaleTransition;
		[SerializeField]
		private Material separatingLine;
		
		[SerializeField]
		private AnimationCurve showingCurve;
		[SerializeField]
		private AnimationCurve hidingCurve;
		
		[SerializeField]
		private List<ScriptableRendererFeature> effectsFeatures;
		
		private Coroutine currentAnimation;
		
		private float progress = 0;
		private bool isShown;
		
		private void OnDisable()
		{
			for (var i = 0; i < this.effectsFeatures.Count; i++)
			{
				this.effectsFeatures[i].SetActive(false);
			}
		}
		
		public void Show()
		{
			this.isShown = true;
			
			for (var i = 0; i < this.effectsFeatures.Count; i++)
			{
				this.effectsFeatures[i].SetActive(true);
			}
			
			if (this.currentAnimation != null)
			{
				this.StopCoroutine(this.currentAnimation);
			}
			
			this.currentAnimation = this.StartCoroutine(this.ShowTransitionAnimation());
		}
		
		public void Hide()
		{
			this.isShown = true;
			
			if (this.currentAnimation != null)
			{
				this.StopCoroutine(this.currentAnimation);
			}
			
			this.currentAnimation = this.StartCoroutine(this.HideTransitionAnimation());
		}
		
		public bool IfShown()
		{
			return this.isShown;
		}
		
		private IEnumerator ShowTransitionAnimation()
		{
			while(this.progress < SceneComponentPlayerDieEffect.MaxEdge)
			{
				this.progress += Time.deltaTime;
				
				this.SetAnimationProgress(this.showingCurve, this.progress);
				
				yield return null;
			}
		}
		
		private IEnumerator HideTransitionAnimation()
		{
			while (this.progress > 0)
			{
				this.progress -= Time.deltaTime;
				
				this.SetAnimationProgress(this.hidingCurve, this.progress);
				
				yield return null;
			}
			
			this.OnDisable();
		}
		
		private void SetAnimationProgress(AnimationCurve animationCurve, float progress)
		{
			float edge = animationCurve.Evaluate(progress / SceneComponentPlayerDieEffect.MaxEdge) * SceneComponentPlayerDieEffect.MaxEdge;
			this.grayscaleTransition.SetFloat(SceneComponentPlayerDieEffect.EdgeKey, edge);
			this.separatingLine.SetFloat(SceneComponentPlayerDieEffect.EdgeKey, edge);
		}
	}
}
