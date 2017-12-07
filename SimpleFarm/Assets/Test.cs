using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Student
{
    public string name { get; set; }
    public string rollNumber { get; set; }

    public Student(string n, string r)
    {
        name = n;
        rollNumber = r;
    }
}

public class Test : MonoBehaviour {

    public bool band;

    // Use this for initialization
    void Start () {
        /*
        band = false;

        Action<int> MyDelegate = Imprimir;

        MyDelegate(2);

        Action<string> MyDelegate2 = delegate (string aux)
        {
            print(aux);
        };

        MyDelegate2("hola");

        Action<float> MyDelegate3 = (float a) => print(a);

        MyDelegate3(1.2f);

        //Whit objects reference

        Student obj;
        obj = new Student("Pedro", "01");

        Action<Student, Exception> action;
            
        action = Display;
        action(obj, null);
        CallingAction(action);

        action = Show;
        action(obj, null);
        CallingAction(action);
        */

        Action action2;

        action2 = () =>
        {
            print("boy");
        };

        StartCoroutine(Coroutine(action2));
    }

    public void Callback()
    {
        print("Callback");
    }

    public void Imprimir(int a)
    {
        print(a);
    }

    public void CallingAction(Action<Student, Exception> s)
    {
        print(s.Method.Name);
    }

    public void Display(Student s, Exception e)
    {
        print("Display: "+ s.name);
        print("Display: " + s.rollNumber);
    }

    public void Show(Student s, Exception e)
    {
        print("Show: " + s.name);
        print("Show: " + s.rollNumber);
    }
	
    public IEnumerator Coroutine(Action a)
    {
        while (true)
        {
            print(band);
            if (band)
            {
                a();
                yield break;
            }

            yield return null;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
