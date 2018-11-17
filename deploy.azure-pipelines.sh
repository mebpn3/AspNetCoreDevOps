#!/bin/sh
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker --version'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker kill dev_api'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker rm dev_api'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker kill dev_elasticsearch'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker rm dev_elasticsearch'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker kill dev_db'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker rm dev_db'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker kill dev_pgadmin'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker rm dev_pgadmin'
sshpass -p $(VmPassword) ssh -o StrictHostKeyChecking=no $(VmUser)@$(VmHost) 'sudo docker-compose -f dev/docker-compose.yml up -d --build'