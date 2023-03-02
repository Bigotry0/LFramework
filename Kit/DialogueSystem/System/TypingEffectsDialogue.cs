using System;
using System.Collections;
using LFramework.Kit.DialogueSystem;
using UnityEngine;

public class TypingEffectsDialogue : DialogueSystem
{
    /// <summary>
    /// 文本打印速度
    /// </summary>
    public float TypingSpeed;

    /// <summary>
    /// 缓存当前对话文本
    /// </summary>
    private string CurrentSentence = String.Empty;

    /// <summary>
    /// 缓存对话打印协程
    /// </summary>
    private Coroutine TextEffectCoroutine;

    protected override void PlayText(string sentence)
    {
        //打印前将对话状态设定为IsPlayed
        PlayTextState = PlayTextStates.IsPlayed;

        CurrentSentence = sentence;
        TextEffectCoroutine = StartCoroutine(StartPlayText());
    }

    protected override void NextOnTextIsPlayed()
    {
        StopCoroutine(TextEffectCoroutine);
        OnPlayText?.Invoke(CurrentSentence);

        //打印完成后将对话状态设定为Finished
        PlayTextState = PlayTextStates.Finished;
    }

    /// <summary>
    /// 打字机效果
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartPlayText()
    {
        string sentenceToPlay = string.Empty;
        var length = CurrentSentence.Length;

        for (var i = 0; i <= length; i++)
        {
            yield return new WaitForSeconds(TypingSpeed);

            sentenceToPlay = CurrentSentence.Substring(0, i);
            OutputText(sentenceToPlay);
            //OnPlayText?.Invoke(sentenceToPlay);
        }

        //打印完成后将对话状态设定为Finished
        PlayTextState = PlayTextStates.Finished;
    }
}