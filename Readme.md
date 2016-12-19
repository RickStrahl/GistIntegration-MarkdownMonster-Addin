# Paste Code as Gist Markdown Monster Add-in

<img src="Build\icon.png" width="128" />

This project provides a [Markdown Monster](https://markdownmonster.west-wind.com) Add-in for pasting code into a Markdown document based on a Gist that you create and post to Github, and then embed a link to - via `<script>` tag into the Markdown document.

> #### @icon-info-circle EditorAllowRenderScriptTags
> In order for this addin to work you need to enable the **EditorAllowRenderScriptTags** flag in the Markdown Monster settings.

This addin is still a bit rough in its early release and there's no configuration UI. In order to configure the addin credentials if you want to post non-anonymous Gists you can edit the `PasteCodeAsGistAddin.json` file.

```json
{
  "GithubUserToken": "12345e0deb0c66041719d4cc7dec6cd45e",
  "GithubUsername": "RickStrahl"
}
```