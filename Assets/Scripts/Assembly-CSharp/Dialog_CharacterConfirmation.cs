using UnityEngine;

public class Dialog_CharacterConfirmation : MonoBehaviour
{
	public static void Display(Characters.Type character)
	{
		DialogStack.ShowDialog("Character Confirmation Dialog");
	}

	private void Start()
	{
	}
}
