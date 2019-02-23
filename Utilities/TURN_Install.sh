# Arguments are username, password, realm.
apt-get update 
apt-get upgrade 
apt-get install -y coturn

echo "TURNSERVER_ENABLED=1" | tee /etc/default/coturn

echo "listening-port=3478
realm=$3
server-name=$3
lt-cred-mech
" | tee /etc/turnserver.conf
 
turnadmin -a -u $1 -p $2 -r $3