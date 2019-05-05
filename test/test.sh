  
    runTests()
    {
      testsName=$1
      testsFolder=$2
      configuration=$3

      cd $testsFolder
      echo Running tests in $testsFolder
      testOutputFile="$testsName"Output.txt
      dotnet test --no-build --configuration $configuration --no-restore --logger trx > $testOutputFile
      TestResult=$?
      
     # echo Looking for a tests results
     # cd ./bin/$configuration/netcoreapp2.0/Logs
     # find . -name '*.trx' -exec cp {} .
      zip -r ./"$testsName"_logs.zip ./TestResults/*.trx
       
      #cd -
      cd ..

      if [ $TestResult -ne 0 ];then
         echo should exit...
            #exit #$TestResult
      fi
      echo continue 

    }

   
   cd ../src

   runTests UnitTests GridDomain.Node.Tests Release
   runTests AcceptanceTests GridDomain.Aggregates.Tests Release
   runTests ScenarioTests GridDomain.Scenarios.Tests Release
   runTests ClusterUnitTests GridDomain.Cluster.Tests Release