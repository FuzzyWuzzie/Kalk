using System;
using Gtk;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{
	private Gtk.TreeViewColumn equationColumn, resultColumn, varNameColumn, varValueColumn;
	private Gtk.ListStore equationHistoryStore, varInspectorStore;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		// set up our notebook

		// set up our equation editor
		widget_equationEditor.GrabFocus ();
		widget_equationEditor.Text = "";
		Pango.FontDescription fontDesc = new Pango.FontDescription ();
		fontDesc.Family = "Sans";
		fontDesc.Size = (int)(18.0 * Pango.Scale.PangoScale);
		fontDesc.Weight = Pango.Weight.Normal;
		widget_equationEditor.ModifyFont (fontDesc);

		// our calculation results
		widget_calculationResult.Xalign = 1f;
		widget_calculationResult.ModifyFont (fontDesc);

		// and our equation history
		equationColumn = new Gtk.TreeViewColumn ();
		equationColumn.Title = "Equation";
		resultColumn = new Gtk.TreeViewColumn ();
		resultColumn.Title = "Result";
		widget_equationHistory.AppendColumn (equationColumn);
		widget_equationHistory.AppendColumn (resultColumn);

		// create the text cell to display it
		Gtk.CellRendererText equationHistoryCell = new Gtk.CellRendererText ();
		Gtk.CellRendererText resultHistoryCell = new Gtk.CellRendererText ();
		// add the cell to the column
		equationColumn.PackStart (equationHistoryCell, true);
		resultColumn.PackStart (resultHistoryCell, true);

		// tell the tree view wihich items to display
		equationColumn.AddAttribute (equationHistoryCell, "text", 0);
		resultColumn.AddAttribute (resultHistoryCell, "text", 1);

		// apply our model
		equationHistoryStore = new Gtk.ListStore (typeof(string), typeof(double));
		widget_equationHistory.Model = equationHistoryStore;

		// set up our variable inspector
		varNameColumn = new Gtk.TreeViewColumn ();
		varNameColumn.Title = "Variable";
		varValueColumn = new Gtk.TreeViewColumn ();
		varValueColumn.Title = "Value";
		widget_variableInspector.AppendColumn (varNameColumn);
		widget_variableInspector.AppendColumn (varValueColumn);

		// create the text cell to display it
		Gtk.CellRendererText varNameCell = new Gtk.CellRendererText ();
		Gtk.CellRendererText varValueCell = new Gtk.CellRendererText ();
		varNameColumn.PackStart (varNameCell, true);
		varValueColumn.PackStart (varValueCell, true);
		varNameColumn.AddAttribute (varNameCell, "text", 0);
		varValueColumn.AddAttribute (varValueCell, "text", 1);
		varInspectorStore = new Gtk.ListStore (typeof(string), typeof(double));
		widget_variableInspector.Model = varInspectorStore;

		// add all our variables to the variable inspector
		foreach (KeyValuePair<string, double> entry in Kalk.Variables.variables) {
			varInspectorStore.AppendValues (entry.Key, entry.Value);
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnAbout (object sender, EventArgs e)
	{
		MessageDialog aboutDialogue = new MessageDialog (
			this,
			DialogFlags.Modal | DialogFlags.DestroyWithParent,
			MessageType.Info,
			ButtonsType.Close,
			"Kalk (c) 2014 Kenton Hamaluik\nkenton@hamaluik.com\nhamaluik.com");
		aboutDialogue.Run ();
		aboutDialogue.Destroy ();
	}

	protected void OnQuit (object sender, EventArgs e)
	{
		Application.Quit ();
	}

	protected string fixBrackets (string expression)
	{
		// count the number of opening brackets
		int numOpening = 0, numClosing = 0;
		foreach (char c in expression) {
			if (c == '(')
				numOpening++;
			else if (c == ')')
				numClosing++;
		}

		// if we have more closing than opening, error
		if (numClosing > numOpening)
			throw new Kalk.EquationParseException ("More closing braces than opening ones!");

		// if we have fewer closing than opening, add closing ones
		int delta = numOpening - numClosing;
		for (int i = 0; i < delta; i++)
			expression += ")";
		return expression;
	}

	protected void OnSolve (object sender, EventArgs e)
	{
		// get the expression from the equation editor
		string expression = widget_equationEditor.Text;

		// check exceptions for parsing errors
		double result = 0;
		try {
			// fix the brackets

			// transform it into postfix notation
			expression = Kalk.InfixToPostix.Transform (expression);

			// and solve!
			result = Kalk.EvaluatePostfix.Transform (expression);
		} catch (Exception exc) {
			widget_statusBar.Push (widget_statusBar.GetContextId ("error"), "Error: " + exc.Message);
			return;
		}

		// show the result in the result window
		// TODO: engineering, scientific notation, etc
		widget_calculationResult.Text = result.ToString ();

		// add to our history
		equationHistoryStore.AppendValues (widget_equationEditor.Text, result);

		// clear our equation editor
		widget_equationEditor.Text = "";

		// clear our status bar
		widget_statusBar.Push (widget_statusBar.GetContextId ("ok"), "Success");
	}

	protected void OnCopyResult (object sender, EventArgs e)
	{
		Gtk.Clipboard clipboard = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
		clipboard.Text = widget_calculationResult.Text;
	}

	protected void OnStoreResult (object sender, EventArgs e)
	{
		Gtk.Menu menuBox = new Gtk.Menu ();
		foreach (string variable in Kalk.Variables.variables.Keys) {
			Gtk.MenuItem storeInVar = new Gtk.MenuItem (variable);
			storeInVar.Activated += delegate(object btnSender, EventArgs eventArgs) {
				// store the value
				double value = double.Parse (widget_calculationResult.Text);
				Kalk.Variables.variables [variable] = value;

				// update the list view
				Gtk.TreeIter iter;
				bool valid = varInspectorStore.GetIterFirst (out iter);
				while (valid) {
					if (varInspectorStore.GetValue (iter, 0).Equals (variable)) {
						varInspectorStore.SetValue (iter, 1, value);
						break;
					}
					valid = varInspectorStore.IterNext (ref iter);
				}
			};
			menuBox.Append (storeInVar);
		}
		menuBox.ShowAll ();
		menuBox.Popup ();
	}

	protected void OnVariableClicked (object o, RowActivatedArgs args)
	{
		// figure out what was clicked on
		Gtk.TreeIter iter;
		varInspectorStore.GetIter (out iter, args.Path);

		// get it's name and insert it
		string varName = varInspectorStore.GetValue (iter, 0).ToString ();

		/*if (widget_equationEditor.CursorPosition > 0
			&& widget_equationEditor.Text [widget_equationEditor.CursorPosition] != ' ')
			widget_equationEditor.InsertText (" " + varName);
		else
			*/
		//widget_equationEditor.InsertText (varName);
		// TODO: insert text at the cursor instead of appending!
		int cursorPosition = widget_equationEditor.CursorPosition;
		//Console.Write ("Cursor pos: " + cursorPosition);
		widget_equationEditor.InsertText (varName, ref cursorPosition);
		//Console.WriteLine (" -> " + cursorPosition);
		//Console.WriteLine (.ToString ());
		//widget_equationEditor.Text += varName;
		/*widget_equationEditor.GrabFocus ();
		widget_equationEditor.SelectRegion (cursorPosition, cursorPosition);*/
	}
}
