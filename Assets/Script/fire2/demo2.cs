using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class PositionSaver : MonoBehaviour
{
    // Reference to Firebase Database
    private DatabaseReference reference;

    // GameObject whose position you want to save/load
    public GameObject targetObject;

    // The username is fetched from PlayerPrefs
    private string username;

    void Start()
    {
       
        username = PlayerPrefs.GetString("username", "");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is not set. Please set a username first.");
            return;
        }

       
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;

           
            LoadPosition();
        });
    }

   
    public void SavePosition()
    {
        if (!string.IsNullOrEmpty(username) && targetObject != null && reference != null)
        {
            
            Vector3 position = targetObject.transform.position;

          
            var positionData = new Dictionary<string, object>
            {
                { "x", position.x },
                { "y", position.y },
                { "z", position.z }
            };

           
            reference.Child("positions").Child(username).SetValueAsync(positionData).ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Position saved to Firebase successfully for user: " + username);
                }
                else
                {
                    Debug.LogError("Failed to save position to Firebase: " + task.Exception);
                }
            });
        }
    }

    
    public void LoadPosition()
    {
        if (!string.IsNullOrEmpty(username) && targetObject != null && reference != null)
        {
            
            reference.Child("positions").Child(username).GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        
                        float x = float.Parse(snapshot.Child("x").Value.ToString());
                        float y = float.Parse(snapshot.Child("y").Value.ToString());
                        float z = float.Parse(snapshot.Child("z").Value.ToString());

                       
                        targetObject.transform.position = new Vector3(x, y, z);
                        Debug.Log("Position loaded from Firebase for user: " + username);
                    }
                    else
                    {
                        Debug.Log("No position data found in Firebase for user: " + username);
                    }
                }
                else
                {
                    Debug.LogError("Failed to load position from Firebase: " + task.Exception);
                }
            });
        }
    }
}
