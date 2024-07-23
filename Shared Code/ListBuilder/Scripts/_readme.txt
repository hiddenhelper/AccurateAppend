This is a note for others coming behind in the codebase for how the ListBuilder scripts work.
All the *.ts and *.js.Map content is added to the appropriate folder in your desired project
as a linked item referring to the shared sln content. These files must be shown as part of the
project content for the WebDeploy command to push them out to the deployment location. At runtime
in VS however, a copy of te generated JS must also be found in the actual script folder under
your project path. Therefore a build event on your project is added that On Sucessful Build
will copy this generated content from the shared location into the project folder such as.

copy "$(SolutionDir)Shared Code\ListBuilder\Scripts\*.js" "$(SolutionDir){YOUR PROJECT NAME}\Areas\{YOUR AREA}\Scripts"

Your HTML code should reference the scripts via the path /Area/{YOUR AREA}/Scripts/{YOUR SCRIPT}.js

As MVC 5.* and newer change the pipeline behavior, it will likely be required to specifically allow
the js files to be run outside of the MVC routest via the static handler. If needed, add the following
entry to your area view web.config file.

<add name="Allow Scripts" path="*.js" verb="*" type="System.Web.StaticFileHandler" />