  
    runTests()
    {
      testsName=$1
      testsFolder=$2
      configuration=$3

      cd $testsFolder
      echo Running tests in $testsFolder
      dotnet test --no-build --configuration $configuration --no-restore --logger trx > TestsOutput.txt
      TestResult=$?
      
      echo Looking for a tests results
      cd ./bin/$configuration/netcoreapp2.0/Logs
      zip -r ./"$testsName"Logs.zip ./*
       
      cd -
      cd ..

      if [ $TestResult -ne 0 ];then
         echo should exit...
            #exit #$TestResult
      fi
      echo continue 

    }