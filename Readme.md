# Gist Integration Markdown Monster Add-in

<img src="Build\icon.png" width="128" />

A [Markdown Monster](https://markdownmonster.west-wind.com) addin that integrates with a couple of Gist features:

* Paste Code as Gists and Embed them into the Document
* Open Documents (markdown or otherwise) from Gist
* Save Documents (markdown or otherwise) to Gist

that takes the current Clipboard or Editor text selection and pastes it as a Gist on Github. The resulting Gist is then embedded - via `<script>` tag - into the current Markdown document replacing the current selection.

### Usage for Paste Code as Gist
Paste Code as Gist UI:

![Paste Code as Gist Addin UI](screenshot.png)

Here's the full set of steps to create and embed a Gist:

* Create some code in the editor
* Align code the way you want it (typically all the way to left)
* Select the code
* *Alternately* copy any external code to your Clipboard
* Click the Gist icon in toolbar
* Your code shows up in the editor
* Assign a filename with the extension that matches your code (ie. C# == MyFile.cs)
* Add an optional description
* Click Paste Code

The addin uses the Gist REST API to post the code to Github. The Gist API creates the Gist and returns a URL to the Gist so you can embed it into the page as an embedded Gist code block.

The embedded code in the markdown creates a `<script>` tag like this:

```html
<script src="https://gist.github.com/35c288114e2cd98e1ca4fd875e7749fe.js"></script>
```

> #### Set *AllowRenderScriptTags: true*
> Markdown Monster by default doesn't allow `<script>` tags to be rendered. Embedded Gists use `<script>` tags and so you **have to** enable script embedding by setting the `AllowRenderScriptTags: true` in the Markdown Monster settings. Goto **Tools -> Settings** and find the **AllowRenderScriptTags** key to set.

### Usage for Open and Saving Documents from Gists
Open from and Save to Gist UI:

![Open from and Save to Gist UI](Screenshot2.png)

The addin adds two new Menu Options on the File menu:

* Open from Gist
* Save to Gist

To open a document from a Gist simply use **Open from Gist**. Use the selection list to pick any Gist you want to load. Alternately you can explicitly type in a Gist Id. The username can be any user name, but it'll default the configuration value for `GithubUsername` in the Addin configuration. You can open Markdown documents or any other kind of code document for editing.

To Save a Document to a Gist use **Save to Gist** while a document is open. You can use the list to select a Gist from the list to update an existing Gist, or check **Save as New Gist** checkbox to create a new Gist with a filename.

> #### File Name Extensions determine Syntax Coloring for Gist
> For new Gists, you should **always** create a filename with an appropriate extension for the file you're saving so the Gist can display with the proper syntax highlighting.

Click the **Save** button to upload your Gist.

### Configuration
In order to create new Gists you need to have a **GithubUserToken** and set it in the add-in's configuration. To configure the addin click on the down arrow next to the Gist icon on the toolbar can click **Gist Configuration**. This edits **%AppData%\Markdown Monster\PasteCodeAsGistAddin.json**.

In the file add or edit these values:

```json
{
  "GithubUserToken": "12345e0deb0c66041719d4cc7dec6cd45e",
  "GithubUsername": "RickStrahl"
}
```

If the user token isn't set, all gists are posted anonymously without association to your account.
