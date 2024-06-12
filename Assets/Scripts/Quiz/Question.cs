using System;
using System.Collections.Generic;

[System.Serializable] // Makes the class serializable for Unity
public class Question
{
    public string questionText; // Text of the question
    public string[] answers; // Array of possible answers
    public int correctAnswerIndex; // Index of the correct answer in the answers array
    public string category;
    public int id;

    // You can add constructors or methods if needed
}
[System.Serializable]
public class QuestionList
{
    public Question[] questions; // An array of questions
}