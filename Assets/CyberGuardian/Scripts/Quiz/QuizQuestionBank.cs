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
        public string title = "QUIZ NODE";
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
            CyberQuestionCategory requestedCategory = (CyberQuestionCategory)Mathf.Abs(category % 4);
            List<QuizQuestion> matches = new List<QuizQuestion>();

            for (int i = 0; i < questions.Count; i++)
            {
                QuizQuestion question = questions[i];
                if (question != null && question.IsUsable() && question.category == requestedCategory)
                {
                    matches.Add(question);
                }
            }

            if (matches.Count > 0)
            {
                return matches[Mathf.Abs(seed) % matches.Count];
            }

            if (fallbackQuestions != null && fallbackQuestions.Count > 0)
            {
                return fallbackQuestions[Mathf.Abs(category) % fallbackQuestions.Count];
            }

            return null;
        }
    }
}
