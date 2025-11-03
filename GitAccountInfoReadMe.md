**!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"UPDATE THIS FILE IF ANY CREDENTIAL CHANGED"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!**



**GitHub login \&\& BesiSgPackaging email login:**

username: besisgpackaging2025@outlook.com

pass	: besi2025!



**GitHub Developer token:**

ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX



**GitHub Developer token (fine grained):**

github\_pat\_11BZL4EJA0fu2T5AP3Cuhv\_P83KuFzg92ojnYdauTaDUQa0kJ2wH9OT6owmwDkIvyZKN7BKRKUFOjx15BX



**curl command to create new repositories in GitHub:**

curl -H "Authorization: Bearer ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX" -X POST -H "Content-Type: application/json" -d "{\\"name\\":\\"NCP\_UI\\",\\"description\\":\\"NCP\_UI\\",\\"private\\":false}" https://api.github.com/user/repos



**Initial commit on a new repo (p.s. Github must already have that existing Repositories, in order for us to push):** 

git init

git add .

git commit -m "first commit"

git branch -M main

Format: git remote set-url origin https://<PAT>@<HOST>/<USERNAME>/<REPOSITORY>.git

git remote add origin https://ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX@github.com/BesiSg/IssueTrackernet_AI.git

git remote set-url origin https://ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX@github.com/BesiSg/IssueTrackernet_AI.git

git push -u origin main



**Commit new changes:**

git remote add origin https://ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX@github.com/BesiSg/IssueTrackernet_AI.git

git remote set-url origin https://ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX@github.com/BesiSg/IssueTrackernet_AI.git

git add .

git commit -m "commit message"

git push origin main



**Discard local changes and pull in latest commit:**

git reset --hard

git pull origin main



**Pull from GitHub with empty repo:**

cd to/your/local/repo

git init

git remote add origin https://ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX@github.com/BesiSg/IssueTrackernet_AI.git

git remote set-url origin https://ghp\_smRD3AoobBDWSMHzNkI5057xqKAMec22KLdX@github.com/BesiSg/IssueTrackernet_AI.git

git pull origin main

