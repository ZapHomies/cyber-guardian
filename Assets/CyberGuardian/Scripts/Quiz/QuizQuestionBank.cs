using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberGuardian
{
    public enum CyberQuestionCategory
    {
        Password = 0,
        Malware = 1,
        Network = 2,
        Privacy = 3
    }

    [Serializable]
    public sealed class QuizQuestion
    {
        public CyberQuestionCategory category;
        [Range(0, 3)] public int lessonTier;
        public string title = "NODE KUIS";
        [TextArea(2, 5)] public string prompt = "Pertanyaan keamanan siber";
        public string[] answers = { "Jawaban A", "Jawaban B", "Jawaban C", "Jawaban D" };
        [Range(0, 3)] public int correctIndex;
        [TextArea(2, 4)] public string feedback = "Jawaban benar.";

        public QuizQuestion()
        {
        }

        public QuizQuestion(CyberQuestionCategory category, string title, string prompt, string[] answers, int correctIndex, string feedback)
        {
            this.category = category;
            this.title = title;
            this.prompt = prompt;
            this.answers = answers;
            this.correctIndex = Mathf.Clamp(correctIndex, 0, 3);
            this.feedback = feedback;
        }

        public QuizQuestion(int lessonTier, CyberQuestionCategory category, string title, string prompt, string[] answers, int correctIndex, string feedback)
            : this(category, title, prompt, answers, correctIndex, feedback)
        {
            this.lessonTier = Mathf.Clamp(lessonTier, 0, 3);
        }

        public bool IsUsable()
        {
            return !string.IsNullOrWhiteSpace(prompt) && answers != null && answers.Length >= 2;
        }
    }

    [CreateAssetMenu(menuName = "Cyber Guardian/Quiz Question Bank", fileName = "QuizQuestionBank")]
    public sealed class QuizQuestionBank : ScriptableObject
    {
        public List<QuizQuestion> questions = new List<QuizQuestion>();

        public QuizQuestion GetQuestion(int category, int seed, IReadOnlyList<QuizQuestion> fallbackQuestions)
        {
            return GetQuestion(category, 0, seed, fallbackQuestions);
        }

        public QuizQuestion GetQuestion(int category, int lessonTier, int seed, IReadOnlyList<QuizQuestion> fallbackQuestions)
        {
            CyberQuestionCategory requestedCategory = (CyberQuestionCategory)Mathf.Abs(category % 4);
            int requestedTier = Mathf.Clamp(lessonTier, 0, 3);
            List<QuizQuestion> matches = new List<QuizQuestion>();

            for (int i = 0; i < questions.Count; i++)
            {
                QuizQuestion question = questions[i];
                if (question != null && question.IsUsable() && question.category == requestedCategory && (requestedTier == 0 || question.lessonTier == requestedTier || question.lessonTier == 0))
                {
                    matches.Add(question);
                }
            }

            if (matches.Count > 0)
            {
                int index = (seed & 0x7fffffff) % matches.Count;
                return matches[index];
            }

            if (fallbackQuestions != null && fallbackQuestions.Count > 0)
            {
                int fallbackIndex = (category & 0x7fffffff) % fallbackQuestions.Count;
                return fallbackQuestions[fallbackIndex];
            }

            return null;
        }
    }
}
